#include "Email.h"

namespace Backend::Authentication
{

    Email::Email(std::string recipient, std::string subject) : m_recipient(recipient), m_subject(subject)
    {
    }

    Email::~Email()
    {
    }

    void Email::AddLine(std::string line)
    {
        m_emailText += line + "\n";
    }
}