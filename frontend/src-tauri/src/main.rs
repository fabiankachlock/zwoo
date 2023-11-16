#![cfg_attr(
    all(not(debug_assertions), target_os = "windows"),
    windows_subsystem = "windows"
)]

use tauri::{Menu, MenuItem, Submenu};

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
        // .menu(menu)
        .run(tauri::generate_context!())
        .expect("error while running tauri application");
}
