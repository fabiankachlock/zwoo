use std::io::{BufRead, BufReader};
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
            args.push("--port".to_string());
            args.push(format!("{}", self.config.port));
        }

        if !self.config.server_id.is_empty() {
            args.push("--server-id".to_string());
            args.push(self.config.server_id.to_string());
        }
        if !self.config.secret_key.is_empty() {
            args.push("--secret-key".to_string());
            args.push(self.config.secret_key.to_string());
        }

        if !self.config.allowed_origins.is_empty() {
            args.push("--allowed-origin".to_string());
            args.push(self.config.allowed_origins.to_string());
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

    pub fn start(&mut self) -> Result<bool, String> {
        if let Some(_child) = self.child.take() {
            println!("[app] server is already running!");
            return Ok(false);
        }

        println!("[app] starting server with args: {:?}", self.build_args());

        // strip unc path prefix
        println!("stripping path: {:?}", self.path.as_str());
        let mut stripped_path = self.path.strip_prefix("\\\\?\\");
        if stripped_path != Some(self.path.as_str()) && stripped_path != None {
            println!("[app] stripped path to: {:?}", stripped_path.unwrap());
        } else {
            stripped_path = Some(self.path.as_str());
        }

        let cmd = Command::new(stripped_path.unwrap())
            .args(self.build_args())
            .stdout(Stdio::piped())
            .spawn();

        if let Err(e) = cmd {
            println!("[app] error starting server: {}", e);
            return Err("tauri.server.cantStart".to_string().to_string());
        }

        let mut cmd = cmd.unwrap();
        let mut out = cmd.stdout.take();

        self.child = Some(cmd);
        println!("[app] server started");

        tokio::spawn(async move {
            if let Some(ref mut stdout) = out {
                let reader = BufReader::new(stdout);

                let mut lines_stream = reader.lines().map(|line| line.unwrap());
                while let Some(line) = lines_stream.next() {
                    println!("[srv] {}", line);
                }
            }
        });
        Ok(true)
    }

    pub fn stop(&mut self) -> Result<bool, String> {
        if let Some(mut child) = self.child.take() {
            println!("[app] server is stopping!");
            if let Err(e) = child.kill() {
                println!("[app] error stopping server: {}", e);
                return Err("tauri.server.cantStop".to_string());
            }

            self.child = None;
            println!("[app] server stopped!");
            return Ok(true);
        }

        println!("[app] server is not running");
        Ok(false)
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
