use std::process::{Child, Command};

pub struct Server {
    child: Option<Child>,
    path: String,
}

impl Server {
    pub fn new(path: String) -> Server {
        Server { child: None, path }
    }

    pub fn start(&mut self) {
        if let Some(_child) = self.child.take() {
            println!("Server is already running!");
            return;
        }

        self.child = Some(
            Command::new(self.path.as_str())
                .spawn()
                .expect("failed to start server"),
        );
        println!("Server started")
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
}
