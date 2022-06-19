#pragma once

// (c)VisualRock

#include <functional>
#include <initializer_list>

template <class T> struct Node
{
    Node( T val, Node<T> *p, Node<T> *n ) : value( val ), prev( p ), next( n )
    {
    }

    T value;
    Node<T> *prev;
    Node<T> *next;
};

template <class T> class CyclingList
{
  public:
    class iterator
    {
      public:
        iterator( Node<T> *c ) : node( c ) {}

        iterator &operator++( )
        {
            node = node->next;
            return *this;
        }
        iterator &operator--( )
        {
            node = node->prev;
            return *this;
        }
        T &operator*( ) { return node->value; }

        bool operator==( const iterator &rhs ) const
        {
            return node == rhs.current;
        }
        bool operator!=( const iterator &rhs ) const
        {
            return node != rhs.current;
        }

        Node<T> *node;
    };

  public:
    CyclingList( ) : m_first( nullptr ), m_last( nullptr ), m_size( 0 ) {}
    CyclingList( std::initializer_list<T> init_list )
        : m_first( nullptr ), m_last( nullptr ), m_size( 0 )
    {
        for ( auto elem : init_list )
        {
            push_back( elem );
        }
    }
    ~CyclingList( )
    {
        Node<T> *node = m_first;
        Node<T> *del;
        for ( std::size_t i = 0; i < m_size; i++ )
        {
            del = node;
            node = node->next;
            delete del;
        }
    }

    inline std::size_t size( ) { return m_size; }

    auto for_each( std::function<void( std::size_t *, T )> exp ) -> void
    {
        Node<T> *node = m_first;
        for ( std::size_t i = 0; i < m_size; i++ )
        {
            exp( &i, node->value );
            node = node->next;
        }
    }
    auto for_each_r( std::function<void( std::size_t *, T )> exp ) -> void
    {
        Node<T> *node = m_last;
        for ( std::size_t i = 0; i <= m_size; i++ )
        {
            exp( &i, node->value );
            node = node->prev;
        }
    }

    auto find_first( const T &val ) -> iterator
    {
        Node<T> *node = m_first;
        for ( std::size_t i = 0; i < m_size; i++ )
        {
            if ( node->value == val )
            {
                return iterator( node );
                break;
            }
            node = node->next;
        }
        return iterator( new Node<T>( T( ), nullptr, nullptr ) );
    }
    auto find_last( const T &val ) -> iterator
    {
        Node<T> *node = m_last;
        for ( std::size_t i = 0; i < m_size; i++ )
        {
            if ( node->value == val )
            {
                return iterator( node );
                break;
            }
            node = node->prev;
        }
        return iterator( new Node<T>( T( ), nullptr, nullptr ) );
    }

    auto push_back( T elem ) -> void
    {
        auto n = new Node<T>( elem, m_last, m_first );
        if ( m_first == nullptr || m_last == nullptr )
        {
            n->next = n;
            n->prev = n;
            m_first = n;
            m_last = n;
        }
        else
        {
            m_last->next = n;
            m_first->prev = n;
            m_last = n;
        }
        m_size++;
    }
    auto pop_back( ) -> void
    {
        Node<T> *n;
        if ( size( ) == 0 )
            return;
        n = m_last;
        if ( size( ) == 1 )
        {
            m_first = nullptr;
            m_last = nullptr;
            m_size--;
        }
        else
        {
            m_last->prev->next = m_first;
            m_first->prev = m_last->prev;
            m_last = m_last->prev;
            m_size--;
        }
        delete n;
    }

    auto erase( iterator n ) -> void
    {
        n.node->prev->next = n.node->next;
        n.node->next->prev = n.node->prev;
        delete n.node;
        m_size--;
    }
    auto insert_befor( iterator it, T val ) -> void
    {
        Node<T> *n = new Node<T>( val, it.node->prev, it.node );
        it.node->prev->next = n;
        it.node->prev = n;
    }
    auto insert_after( iterator it, T val ) -> void
    {
        Node<T> *n = new Node<T>( val, it.node, it.node->next );
        it.node->next->prev = n;
        it.node->next = n;
    }

    auto first( ) -> iterator { return iterator( m_first ); }
    auto last( ) -> iterator { return iterator( m_last ); }

  private:
    Node<T> *m_first;
    Node<T> *m_last;

    std::size_t m_size;
};