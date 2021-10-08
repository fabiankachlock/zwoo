sudo apt update -y
sudo apt upgrade -y

# install cmake
sudo apt install cmake -y
sudo apt install build-essential -y
wget https://github.com/curl/curl/releases/download/curl-7_79_1/curl-7.79.1.tar.gz
sudo apt install git -y

git clone https://github.com/fabiankachlock/zwoo.git --recursiv

cd zwoo/backend/
cmake .
make

