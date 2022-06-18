using ClusterioLibSharp.NodeLibs;
using Json.Schema;
using Newtonsoft.Json;
using SharpPromise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClusterioLibSharp.Link
{
  public class MissingLinkhandlerError : Exception
  {
    string type;
    string source;
    string target;
    string handler;
    string plugin;

    public MissingLinkhandlerError(string type, string source, string target) : base()
    {
      this.type = type;
      this.source = source;
      this.target = target;
    }

    public override string Message {
      get {
        string handlerText = handler ?? "handler";
        string pluginText = plugin != null ? $" on plugin {plugin}" : "";
        return $"Missing {handlerText}{pluginText} for {type} on {source}-{target} link";
      }
    }
  }

  public class Property
  {
    public string type { get; set; }
  }

  public class Request : Message
  {
    private List<string> links;
    private string forwardTo;
    private string handlerSuffix = "RequestHandler";
    private string requestType;
    private string responseType;


    private JsonSchema requestValidatorSchema;
    private JsonSchema responseValidatorSchema;
    private Link.ValidatorCB requestValidator;
    private Link.ValidatorCB responseValidator;

    public delegate dynamic RequestHandler(Link link, Message message, Request request);

    public Request(
      string type,
      List<string> links,
      string forwardTo = null,
      List<string> requestRequired = null,
      Dictionary<string, Property> requestProperties = null,
      List<string> responseRequired = null,
      Dictionary<string, Property> responseProperties = null) : base()
    {
      this.type = type;
      this.links = links;
      this.forwardTo = forwardTo;

      requestType = $"{type}_request";
      responseType = $"{type}_response";

      if (requestRequired == null)
      {
        requestRequired = requestProperties.Keys.ToList();
      }

      if (forwardTo == "instance")
      {
        requestRequired.Insert(0, "instance_id");
        requestProperties.Add("instance_id", new Property() { type = "integer" });
      }
      else if (forwardTo != "master" && forwardTo != null)
      {
        throw new Exception($"Invalid forwardTo value {forwardTo}");
      }

      requestValidatorSchema = JsonSchema.FromText($@"{{
        ""$schema"": ""http://json-schema.org/draft-07/schema#"",
        ""properties"": {{
          ""type"": {{ ""const"": ""{requestType}"" }},
				  ""data"": {{
            ""additionalProperties"": false,
            ""required"": {JsonConvert.SerializeObject(requestRequired)},
            ""properties"": {JsonConvert.SerializeObject(requestProperties)}
          }}
			  }}
      }}");
      // TODO requestValidator
      //requestValidator = (message) => requestValidatorSchema.Validate()

      if (responseRequired == null)
      {
        responseRequired = responseProperties.Keys.ToList();
      }

      var responseProps = new Dictionary<string, Property>(responseProperties);
      responseProps.Add("seq", new Property() { type = "integer" });

      responseValidatorSchema = JsonSchema.FromText($@"{{
        ""$schema"": ""http://json-schema.org/draft-07/schema#"",
        ""properties"": {{
          ""type"": {{ ""const"": ""{responseType}"" }},
				  ""data"": {{
            ""anyOf"": [
              {{
                ""additionalProperties"": false,
                ""required"": {JsonConvert.SerializeObject(Enumerable.Concat(new[] { "seq" }, responseRequired))},
                ""properties"": {JsonConvert.SerializeObject(responseProps)}
              }},
              {{
                ""additionalProperties"": false,
                ""required"": [""seq"", ""error""],
                ""properties"": {{
                  ""seq"": {{ ""type"": ""integer"" }},
                  ""error"": {{ ""type"": ""string"" }}
                }}
              }}
            ]
          }}
			  }}
      }}");
      // TODO responseValidator
    }

    public void attach(Link link, RequestHandler handler)
    {
      if (links.Contains($"{link.source}-{link.target}"))
      {
        link.setValidator(responseType, responseValidator);
      }

      if (!links.Contains($"{link.target}-{link.source}")) return;

      // TODO use forwarder

      if (handler == null)
      {
        throw new MissingLinkhandlerError(requestType, link.source, link.target);
      }

      link.setHandler(requestType, message =>
      {
        new Task<dynamic>(() => handler(link, message, this))
        .AsPromise()
        .Then(response =>
        {
          if (response == null) response = new { };

          dynamic data = DynamicHelpers.CombineDynamics(response, new { seq = message.seq });
          if (!this.responseValidator(new Message(0, responseType, data)))
          {
            //logger.error(JSON.stringify(responseValidator.errors, null, 4)); // TODO
            throw new Exception($"Validation failed responding to {requestType}");
          }

          link.connector.send(responseType, data);
        }).Catch(err =>
        {
          if (!(err is RequestError))
          {
            //logger.error($"Unexpected error while responding to {requestType}:\n{err.stack}"); // TODO
          }
          link.connector.send(responseType, new { seq = message.seq, error = err.Message });
        });
      }, requestValidator);
    }

    // TODO: default data = {}
    public async Task<dynamic> send(Link link, dynamic data)
    {
      if (!requestValidator(new Message(0, requestType, data)))
      {
        //logger.error(JSON.stringify(requestValidator.errors, null, 4)); // TODO
        throw new Exception($"Validation failed sending {requestType}");
      }

      ulong seq = link.connector.send(requestType, data);
      Message responseMessage = await link.waitFor(responseType, new { seq = seq });
      if (responseMessage.data.error)
      {
        throw new RequestError(responseMessage.data.error);
      }
      return responseMessage.data;
    }
  }
}
