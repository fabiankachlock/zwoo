#include <memory>  // for allocator, __shared_ptr_access
#include <string>  // for char_traits, operator+, string, basic_string

#include "ftxui/component/captured_mouse.hpp"  // for ftxui
#include "ftxui/component/component.hpp"       // for Input, Renderer, Vertical
#include "ftxui/component/component_base.hpp"  // for ComponentBase
#include "ftxui/component/component_options.hpp"  // for InputOption
#include "ftxui/component/screen_interactive.hpp"  // for Component, ScreenInteractive
#include "ftxui/dom/elements.hpp"  // for text, hbox, separator, Element, operator|, vbox, border
#include "ftxui/util/ref.hpp"  // for Ref
#include "ftxui/component/component_options.hpp"

#include "CodeGenerator.h" 

int main()
{
    std::string code = "";

    // Input defines
    std::string s_code_amount;
    std::string s_code_format;
    std::string s_db_login_name;
    std::string s_db_login_password;
    std::string s_db_port = "27017";

    bool save_to_file = false;

    ftxui::InputOption _amount = ftxui::InputOption();
    _amount.on_change = [&] {
        s_code_amount.erase(std::remove_if(s_code_amount.begin(), s_code_amount.end(), [] (auto i) { return !std::isdigit(i); }), s_code_amount.end());
    };

    ftxui::InputOption _port = ftxui::InputOption();
    _port.on_change = [&] {
        s_db_port.erase(std::remove_if(s_db_port.begin(), s_db_port.end(), [] (auto i) { return !std::isdigit(i); }), s_db_port.end());
    };

    ftxui::InputOption _pw = ftxui::InputOption();
    _pw.password = true;

    auto i_code_amount = ftxui::Input(&s_code_amount, "amount", _amount);
    auto i_code_format = ftxui::Input(&s_code_format, "format");
    auto i_db_login_name = ftxui::Input(&s_db_login_name, "name");
    auto i_db_login_password = ftxui::Input(&s_db_login_password, "password", _pw);
    auto i_db_port = ftxui::Input(&s_db_port, "port", _port);
    auto i_save_to_file = ftxui::Checkbox("", &save_to_file);
    auto i_generate_codes = ftxui::Button("Generate", [&](){
        if (std::stoi(s_code_amount) > 0 && !s_code_format.empty())
        {
            auto codes = generate_codes(std::stoi(s_code_amount), s_code_format);
            for (auto i : codes)
            {
                code += i;
                code += "    ";
            }
        }
    });

    auto i_save_to_db = ftxui::Button("Save To DB", [](){
        // Save to MongoDB
    });

    auto inputs = ftxui::Container::Vertical({
        i_code_amount,
        i_code_format,
        i_db_login_name,
        i_db_login_password,
        i_db_port,
        i_save_to_file,
        i_generate_codes,
        i_save_to_db
    });

    // Text
    auto codes_text = [&] () {
        auto vbox = ftxui::vbox({});
        return ftxui::paragraph(code);
    };
    

    // Define Layout
    auto main_window = ftxui::Renderer(inputs, [&]{
        return ftxui::hbox({
            /* Sidebar */
            ftxui::vbox({
                ftxui::text("ZWOO Beta codes") | ftxui::bold,
                ftxui::separator(),
                ftxui::hbox(ftxui::text("amount: "), i_code_amount->Render()),
                ftxui::separator(),
                ftxui::hbox(ftxui::text("format: "), i_code_format->Render()),
                ftxui::text(" - x: random char or digit"),
                ftxui::text(" - n: random number 0-9"),
                ftxui::text(" - c: random char a-z, A-Z"),
                ftxui::text(" - C: random char A-Z"),
                ftxui::separator(),
                ftxui::text("Databse Settings:"),
                ftxui::hbox(ftxui::text("name    : "), i_db_login_name->Render()),
                ftxui::hbox(ftxui::text("password: "), i_db_login_password->Render()),
                ftxui::hbox(ftxui::text("port    : "), i_db_port->Render()),
                ftxui::separator(),
                ftxui::text("General"),
                ftxui::hbox(ftxui::text("save to file: "), i_save_to_file->Render()),
                ftxui::separator(),
                i_generate_codes->Render(),
                ftxui::separator(),
                i_save_to_db->Render()
            }) | ftxui::size(ftxui::WIDTH, ftxui::EQUAL, 30),
            ftxui::separator(),
            codes_text() | ftxui::size(ftxui::WIDTH, ftxui::LESS_THAN, 120)
        }) |
        ftxui::border | ftxui::flex_grow;
    });
    
    
    // Start and Render
    auto screen = ftxui::ScreenInteractive::TerminalOutput();
    screen.Loop(main_window);

    return 0;
}
