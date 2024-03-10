use std::process::Stdio;
use std::process::{Child, Command};

use crate::config::LocalServerConfig;

pub struct Server {
    child: Option<Child>,
    path: String,
    pub config: LocalServerConfig,
}

impl Server {
    pub fn new(path: String, config: LocalServerConfig) -> Server {
        Server {
            child: None,
            path,
            config,
        }
    }

    fn build_args(&self) -> Vec<String> {
        let mut args = vec![];
        if self.config.port > 0 {
            args.push(format!("--port {}", self.config.port));
        }

        if !self.config.ip.is_empty() {
            args.push(format!("--server-id {}", self.config.server_id));
        }
        if !self.config.ip.is_empty() {
            args.push(format!("--secret-key {}", self.config.server_id));
        }

        if !self.config.allowed_origins.is_empty() {
            args.push(format!("--allowed-origin {}", self.config.allowed_origins));
        }

        if self.config.use_dynamic_port {
            args.push("--use-dynamic-port".to_string());
        }
        if self.config.use_localhost {
            args.push("--use-localhost".to_string());
        }
        if self.config.use_all_ips {
            args.push("--use-all-ips".to_string());
        }
        if self.config.use_strict_origins {
            args.push("--strict-origins".to_string());
        }
        args
    }

    pub fn start(&mut self) {
        if let Some(_child) = self.child.take() {
            println!("Server is already running!");
            return;
        }

        println!("Starting server with args: {:?}", self.build_args());

        let cmd = Command::new(self.path.as_str())
            .args(self.build_args())
            .stdout(Stdio::piped())
            .spawn()
            .expect("failed to start server");
        // let out = cmd.stdout.unwrap();
        self.child = Some(cmd);
        println!("Server started");

        // let reader = BufReader::new(out);

        // reader
        //     .lines()
        //     .filter_map(|line| line.ok())
        //     .for_each(|line| println!("[srv] {}", line))
    }

    pub fn stop(&mut self) {
        if let Some(mut child) = self.child.take() {
            println!("Server is stopping!");
            match child.kill() {
                Ok(_) => println!("Server stopped!"),
                Err(e) => println!("Error stopping server: {}", e),
            }
            return;
        } else {
            println!("Server is not running");
        }
        self.child = None;
    }

    pub fn is_running(&self) -> bool {
        self.child.is_some()
    }

    pub fn update_config(&mut self, new_config: LocalServerConfig) {
        let old_location = self.config.location.clone();
        self.config = new_config;
        self.config.location = old_location;
        self.config.save();
    }
}
