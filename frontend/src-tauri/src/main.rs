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
fn start_local_server(state: tauri::State<'_, State>) -> Result<bool, String> {
    return state
        .0
        .lock()
        .unwrap()
        .as_mut()
        .ok_or_else(|| "tauri.server.notInitialized".to_string())?
        .start();
}

#[tauri::command]
fn stop_local_server(state: tauri::State<'_, State>) -> Result<bool, String> {
    return state
        .0
        .lock()
        .unwrap()
        .as_mut()
        .ok_or_else(|| "tauri.server.notInitialized".to_string())?
        .stop();
}

#[tauri::command]
fn get_local_server_config(state: tauri::State<'_, State>) -> Result<String, String> {
    let config = state
        .0
        .lock()
        .unwrap()
        .as_mut()
        .ok_or_else(|| "tauri.server.notInitialized".to_string())?
        .config
        .clone();

    Ok(serde_json::to_string(&config).unwrap())
}

#[tauri::command]
fn update_local_server_config(
    state: tauri::State<'_, State>,
    new_config: config::LocalServerConfig,
) -> Result<bool, String> {
    state
        .0
        .lock()
        .unwrap()
        .as_mut()
        .ok_or_else(|| "tauri.server.notInitialized".to_string())?
        .update_config(new_config);

    Ok(true)
}

#[tauri::command]
fn get_server_status(state: tauri::State<'_, State>) -> Result<bool, String> {
    match state.0.lock().unwrap().as_mut() {
        Some(server) => Ok(server.is_running()),
        None => Err("Server not initialized".to_string()),
    }
}

#[tauri::command]
fn open_url(url: String) -> Result<bool, String> {
    match open::that(url) {
        Ok(_) => Ok(true),
        Err(err) => Err(err.to_string()),
    }
}

#[tokio::main]
async fn main() {
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
                .resolve_resource("resources/server/Zwoo.Backend.LocalServer.exe")
                .expect("failed to resolve resource");
            let server_path = resource_path.into_os_string().into_string().unwrap();
            println!("[app] located server executable {}", server_path);

            let config =
                config::load_local_server_config(app.path_resolver().app_data_dir().unwrap());
            println!("[app] loaded server id: {}", config.server_id);
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
            update_local_server_config,
            get_server_status,
            open_url
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
