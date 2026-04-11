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

    // v2
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

    // v3
    void (*set_netwatch_enabled)(uint32_t enabled);
    uint32_t (*is_netwatch_enabled)();
    uint32_t (*get_netwatch_stats)();

    // v4
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

    // v5
    int32_t (*register_custom_employee)(const char* id, const char* name, const char* desc, float salary, float req_rep, uint32_t confirm);
    uint32_t (*is_custom_employee_hired)(const char* id);
    int32_t (*fire_custom_employee)(const char* id);
    int32_t (*register_salary)(int32_t monthly);

    // v6
    int32_t (*show_notification)(const char* msg);
    float (*get_money_per_second)();
    float (*get_expenses_per_second)();
    float (*get_xp_per_second)();
    uint32_t (*is_game_paused)();
    void (*set_game_paused)(uint32_t paused);
    int32_t (*get_difficulty)();
    int32_t (*trigger_save)();

    // v7
    uint64_t (*steam_get_my_id)();
    const char* (*steam_get_friend_name)(uint64_t id);
    // ... other steam functions ...
    void (*reserved[15])(); // Padding for v7 overflow
    void (*get_player_position)(float* x, float* y, float* z, float* ry);

    // v8
    const char* (*payload_get_string)(void* payload, const char* field, const char* fallback);
    void (*subscribe_event)(const char* hook, void (*handler)(void*), const char* mod_id);
    void (*gui_begin_panel)(const char* id, float x, float y, float w, float h);
    void (*gui_label)(const char* text);
    void (*gui_end_panel)();
    int32_t (*raycast_forward)(float max_dist, const char** out_name, float* out_dist, float* out_x, float* out_y, float* out_z);
    void (*publish_tick)(float dt, int32_t frame);
} greg_api_t;

#ifdef __cplusplus
}
#endif

#endif
