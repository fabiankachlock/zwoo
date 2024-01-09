#![cfg_attr(
    all(not(debug_assertions), target_os = "windows"),
    windows_subsystem = "windows"
)]

use std::sync::Mutex;
use tauri::{Manager, Menu, MenuItem, Submenu};

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

fn main() {
    let system_menu = Submenu::new(
        "zwoo",
        Menu::new()
            .add_native_item(MenuItem::EnterFullScreen)
            .add_native_item(MenuItem::Separator)
            .add_native_item(MenuItem::Hide)
            .add_native_item(MenuItem::ShowAll)
            .add_native_item(MenuItem::Separator)
            .add_native_item(MenuItem::CloseWindow)
            .add_native_item(MenuItem::Quit),
    );
    let menu = Menu::new().add_submenu(system_menu);

    tauri::Builder::default()
        .setup(|app| {
            let resource_path = app
                .path_resolver()
                .resolve_resource("../src-server/bin/Release/net8.0/osx-x64/native/src-server")
                .expect("failed to resolve resource");

            let server_path = resource_path.into_os_string().into_string().unwrap();

            println!("located server executable {}", server_path);

            let state = State(Default::default());
            *state.0.lock().unwrap() = Some(exec::Server::new(server_path));
            app.manage(state);
            Ok(())
        })
        .invoke_handler(tauri::generate_handler![
            start_local_server,
            stop_local_server
        ])
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
