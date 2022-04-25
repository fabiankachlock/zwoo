#pragma once

#include <vector>
#include <string>
#include <random>
#include <chrono>

auto generate_codes(int amount, std::string format) -> std::vector<std::string>;
static auto generate_code(std::string format) -> std::string;
static auto random_char() -> std::string;
static auto random_num_char() -> std::string;
static auto random_text_char( bool all_chars) -> std::string;
static auto random_number(int min, int max) -> int;