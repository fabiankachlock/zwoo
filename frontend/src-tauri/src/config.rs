use std::path::PathBuf;

use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize, Debug, Clone)]
pub struct LocalServerConfig {
    pub server_id: String,
    pub port: u16,
    pub ip: String,
    pub use_dynamic_port: bool,
    pub use_localhost: bool,
    pub use_all_ips: bool,
    pub use_strict_origins: bool,
    pub allowed_origins: String,

    #[serde(skip)]
    pub location: PathBuf,
}

pub fn load_app_id(dir: PathBuf) -> String {
    let id_path = dir.join("id.txt");
    let id = if id_path.exists() {
        std::fs::read_to_string(id_path).unwrap()
    } else {
        let id = uuid::Uuid::new_v4().to_string();
        std::fs::write(id_path, &id).unwrap();
        id
    };
    id
}

fn create_default_config(id: String, path: PathBuf) -> LocalServerConfig {
    LocalServerConfig {
        server_id: id,
        port: 8001,
        ip: String::from(""),
        use_dynamic_port: false,
        use_localhost: true,
        use_all_ips: false,
        use_strict_origins: false,
        allowed_origins: String::from(""),
        location: path,
    }
}

pub fn load_local_server_config(dir: PathBuf) -> LocalServerConfig {
    let id = load_app_id(PathBuf::from(&dir));
    let config_path = dir.join("cached-config.json");
    let config = if config_path.exists() {
        let config_str = std::fs::read_to_string(config_path).unwrap();
        let mut config: LocalServerConfig = serde_json::from_str(&config_str).unwrap();
        config.location = dir.clone();
        if config.server_id != id {
            config.server_id = id;
            config.save();
        }
        config
    } else {
        let default_config = create_default_config(id.clone(), dir.clone());
        let config_str = serde_json::to_string_pretty(&default_config).unwrap();
        std::fs::write(config_path, &config_str).unwrap();
        default_config
    };
    config
}

pub fn save_local_server_config(dir: PathBuf, config: LocalServerConfig) {
    let config_path = dir.join("cached-config.json");
    let config_str = serde_json::to_string_pretty(&config).unwrap();
    std::fs::write(config_path, &config_str).unwrap();
}

impl LocalServerConfig {
    pub fn save(&self) {
        save_local_server_config(self.location.clone(), self.clone());
    }
}
