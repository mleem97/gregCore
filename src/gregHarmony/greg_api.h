#ifndef GREG_API_H
#define GREG_API_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef struct {
    uint32_t api_version;
    void (*log_info)(const char* msg);
    void (*log_warning)(const char* msg);
    void (*log_error)(const char* msg);
    double (*get_player_money)();
    void (*set_player_money)(double value);
    float (*get_time_scale)();
    void (*set_time_scale)(float value);
    uint32_t (*get_server_count)();
    uint32_t (*get_rack_count)();
    const char* (*get_current_scene)();

    // v2 - Stats
    double (*get_player_xp)();
    void (*set_player_xp)(double value);
    double (*get_player_reputation)();
    void (*set_player_reputation)(double value);
    float (*get_time_of_day)();
    uint32_t (*get_day)();
    float (*get_seconds_in_full_day)();
    void (*set_seconds_in_full_day)(float value);
    uint32_t (*get_switch_count)();
    uint32_t (*get_satisfied_customer_count)();

    // v3 - NetWatch
    void (*set_netwatch_enabled)(uint32_t enabled);
    uint32_t (*is_netwatch_enabled)();
    uint32_t (*get_netwatch_stats)();

    // v4 - Technicians
    uint32_t (*get_broken_server_count)();
    uint32_t (*get_broken_switch_count)();
    uint32_t (*get_eol_server_count)();
    uint32_t (*get_eol_switch_count)();
    uint32_t (*get_free_technician_count)();
    uint32_t (*get_total_technician_count)();
    int32_t (*dispatch_repair_server)();
    int32_t (*dispatch_repair_switch)();
    int32_t (*dispatch_replace_server)();
    int32_t (*dispatch_replace_switch)();

    // v5 - Employees
    int32_t (*register_custom_employee)(const char* id, const char* name, const char* desc, float salary, float req_rep, uint32_t confirm);
    uint32_t (*is_custom_employee_hired)(const char* id);
    int32_t (*fire_custom_employee)(const char* id);
    int32_t (*register_salary)(int32_t monthly);

    // v6 - Notifications & Systems
    int32_t (*show_notification)(const char* msg);
    float (*get_money_per_second)();
    float (*get_expenses_per_second)();
    float (*get_xp_per_second)();
    uint32_t (*is_game_paused)();
    void (*set_game_paused)(uint32_t paused);
    int32_t (*get_difficulty)();
    int32_t (*trigger_save)();

    // v7 - Steam & Position
    uint64_t (*steam_get_my_id)();
    const char* (*steam_get_friend_name)(uint64_t id);
    int32_t (*steam_create_lobby)(uint32_t type, uint32_t max_players);
    int32_t (*steam_join_lobby)(uint64_t lobby_id);
    void (*steam_leave_lobby)();
    uint64_t (*steam_get_lobby_id)();
    uint64_t (*steam_get_lobby_owner)();
    uint32_t (*steam_get_lobby_member_count)();
    uint64_t (*steam_get_lobby_member_by_index)(uint32_t index);
    int32_t (*steam_set_lobby_data)(const char* key, const char* value);
    const char* (*steam_get_lobby_data)(const char* key);
    int32_t (*steam_send_p2p)(uint64_t target, const void* data, uint32_t len, uint32_t reliable);
    uint32_t (*steam_is_p2p_available)(uint32_t* out_size);
    uint32_t (*steam_read_p2p)(void* buf, uint32_t buf_len, uint64_t* out_sender);
    void (*steam_accept_p2p)(uint64_t remote);
    uint32_t (*steam_poll_event)(void* out_type, void* out_data);
    void (*get_player_position)(float* x, float* y, float* z, float* ry);

    // v8 - Events & UI Core
    const char* (*payload_get_string)(void* payload, const char* field, const char* fallback);
    void (*subscribe_event)(const char* hook, void (*handler)(void*), const char* mod_id);
    void (*gui_begin_panel)(const char* id, float x, float y, float w, float h);
    void (*gui_label)(const char* text);
    void (*gui_end_panel)();
    int32_t (*raycast_forward)(float max_dist, const char** out_name, float* out_dist, float* out_x, float* out_y, float* out_z);
    void (*publish_tick)(float dt, int32_t frame);

    // v9 - Networking (VLAN)
    int32_t (*set_vlan_allowed)(const char* switch_id, int32_t port_index, int32_t vlan_id);
    int32_t (*set_vlan_disallowed)(const char* switch_id, int32_t port_index, int32_t vlan_id);
    int32_t (*is_vlan_allowed)(const char* switch_id, int32_t port_index, int32_t vlan_id);

    // v11 - HUD & Interaction
    void (*hud_update_jade_box)(const char* title, const char* sub_header, const char* entries_json);
    void (*hud_hide_jade_box)();
    int32_t (*get_target_info)(float max_dist, const char** out_type, const char** out_name, float* out_dist, float* out_x, float* out_y, float* out_z);
    const char* (*get_metadata)(float max_dist);
    
    // v12 - UI Hijack (Blueprint Support)
    void (*ui_hijack_canvas)(const char* canvas_name, uint32_t active);
    void* (*ui_create_modern_canvas)(const char* name, int32_t sorting_order);
    void (*ui_set_rect_anchors)(void* transform_ptr, float x_min, float y_min, float x_max, float y_max);
} greg_api_t;

#ifdef __cplusplus
}
#endif

#endif
