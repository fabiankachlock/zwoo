#include "ftxui/component/captured_mouse.hpp"  // for ftxui
#include "ftxui/component/component.hpp"  // for Button, Horizontal, Renderer
#include "ftxui/component/component_base.hpp"      // for ComponentBase
#include "ftxui/component/screen_interactive.hpp"  // for ScreenInteractive
#include "ftxui/dom/elements.hpp"  // for separator, gauge, text, Element, operator|, vbox, border
 

int main()
{
    // Input defines
    std::string s_code_amount = "";
    std::string s_code_format = "xxx-xxx";
    auto i_code_amount = ftxui::Input(&s_code_amount, "amount");
    auto i_code_format = ftxui::Input(&s_code_format, "format");

    auto inputs = ftxui::Container::Vertical({
        i_code_amount
    });

    // Define Layout
    auto main_window = ftxui::Renderer(inputs, [&]{
        return ftxui::hbox({
            ftxui::hbox({
                i_code_amount->Render() | ftxui::border | ftxui::flex_grow,
                i_code_format->Render() | ftxui::border
            })
        }) |
        ftxui::border;
    });
    
    
    // Start and Render
    auto screen = ftxui::ScreenInteractive::TerminalOutput();
    screen.Loop(main_window);

    return 0;
}
