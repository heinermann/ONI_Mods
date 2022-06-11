namespace ClusterioLib
{
  /**
   * Messages to implement (most can just be stubbed):
   * [universal]
   * - prepare_disconnect
   * - ping
   * 
   * [master-slave]
   * - list_saves
   * - create_save
   * - rename_save
   * - copy_save
   * - delete_save
   * - transfer_save
   * - pull_save
   * - push_save
   * - load_scenario
   * - export_data
   * - stop_instance
   * - kill_instance
   * - delete_instance
   * - send_rcon
   * - assign_instance
   * - unassigne_instance
   * - get_metrics
   * - sync_user_lists
   * - banlist_update
   * - adminlist_update
   * - whitelist_update
   * 
   * [slave-master]
   * - update_instances
   * - log_message
   * - instance_status_changed
   * - save_list_update
   * - player_event
   */
  class Messages
  {
  }
}
