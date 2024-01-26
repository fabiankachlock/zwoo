use std::{
    io::{BufReader, Stdout},
    process::{Child, Command},
};

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

        let cmd = Command::new(self.path.as_str())
            .stdout(Stdio::piped())
            .spawn()
            .expect("failed to start server");
        let out = cmd.stdout.unwrap();
        self.child = Some(cmd);
        println!("Server started");

        let reader = BufReader::new(out);

        reader
            .lines()
            .filter_map(|line| line.ok())
            .for_each(|line| println!("[srv] {}", line))
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
