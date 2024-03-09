#![cfg_attr(
    all(not(debug_assertions), target_os = "windows"),
    windows_subsystem = "windows"
)]

use std::sync::Mutex;
use tauri::Manager;

mod config;
mod exec;

#[derive(Default)]
struct State(Mutex<Option<exec::Server>>);

#[tauri::command]
fn start_local_server(state: tauri::State<'_, State>) -> String {
    state
        .0
        .lock()
        .unwrap()
        .as_mut()
        .expect("Server not initialized")
        .start();

    "Server started".to_string()
}

#[tauri::command]
fn stop_local_server(state: tauri::State<'_, State>) -> String {
    state
        .0
        .lock()
        .unwrap()
        .as_mut()
        .expect("Server not initialized")
        .stop();

    "Server stopped".to_string()
}

#[tauri::command]
fn get_local_server_config(state: tauri::State<'_, State>) -> String {
    let config = state.0.lock().unwrap().as_ref().unwrap().config.clone();
    serde_json::to_string(&config).unwrap()
}

#[tauri::command]
fn update_local_server_config(
    state: tauri::State<'_, State>,
    new_config: config::LocalServerConfig,
) -> String {
    state
        .0
        .lock()
        .unwrap()
        .as_mut()
        .expect("Server not initialized")
        .update_config(new_config);

    "Server config updated".to_string()
}

fn main() {
    // let system_menu = Submenu::new(
    //     "zwoo",
    //     Menu::new()
    //         .add_native_item(MenuItem::Hide)
    //         .add_native_item(MenuItem::Separator)
    //         .add_native_item(MenuItem::CloseWindow)
    //         .add_native_item(MenuItem::Quit),
    // );
    // let menu = Menu::new().add_submenu(system_menu);

    tauri::Builder::default()
        .setup(|app| {
            let resource_path = app
                .path_resolver()
                .resolve_resource("../src-server/bin/Release/net8.0/osx-x64/native/src-server")
                .expect("failed to resolve resource");
            let server_path = resource_path.into_os_string().into_string().unwrap();
            println!("[app] located server executable {}", server_path);

            let id = config::load_app_id(app.path_resolver().app_data_dir().unwrap());
            println!("[app] loaded server id: {}", id);

            let config =
                config::load_local_server_config(app.path_resolver().app_data_dir().unwrap());
            println!("[app] loaded last server config: {:?}", config);

            let state = State(Default::default());
            *state.0.lock().unwrap() = Some(exec::Server::new(server_path, config));
            app.manage(state);
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![
            start_local_server,
            stop_local_server,
            get_local_server_config,
            update_local_server_config
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
