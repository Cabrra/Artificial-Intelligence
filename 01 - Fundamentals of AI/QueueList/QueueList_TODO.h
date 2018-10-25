//! \file QueueList_TODO.h
//! \brief Defines the <code>fullsail_ai::data_structures::QueueList</code> class template
//! implementation.
//! \author Cromwell D. Enage
#ifndef _FULLSAIL_AI_FUNDAMENTALS_QUEUELIST_TODO_H_
#define _FULLSAIL_AI_FUNDAMENTALS_QUEUELIST_TODO_H_

namespace fullsail_ai { namespace fundamentals {

	//! \todo
	//!   - Complete the definition of the <code>QueueList::Iterator</code> pre-increment operator.
	//!   - Complete the definition of the <code>QueueList::Iterator</code> dereference operator.
	//!   - Define the <code>QueueList</code> constructor.
	//!   - Define the <code>QueueList</code> destructor.
	//!   - Define the <code>QueueList</code> methods.


	template <typename T>
	typename QueueList<T>::Iterator& QueueList<T>::Iterator::operator++()
	{
		// TODO: Complete the definition of this operator.
		this->currentNode = this->currentNode->next;
		return (*this);
	}

	template <typename T>
	T QueueList<T>::Iterator::operator*() const
	{
		// TODO: Complete the definition of this operator.
		return this->currentNode->data;
	}


	template <typename T>
	bool QueueList<T>::isEmpty() const
	{
		if (back == front && front == nullptr)
			return true;
		else
			return false;
	}

	template <typename T>
	void QueueList<T>::enqueue(T element)
	{
		if (isEmpty())
		{
			front = new Node(element);
			back = front;
		}
		else
		{
			Node* help = new Node(element);
			if (front->next == nullptr)
				front->next = help;
			back->next = help;
			back = help;
		}
	}

	template <typename T>
	T QueueList<T>::getFront() const
	{
		return front->data;
	}

	template <typename T>
	void QueueList<T>::dequeue()
	{
		if (front != nullptr)
		{
			if (back == front)
			{
				delete front;
				front = nullptr;
				back = nullptr;
			}
			else
			{
				Node* help = front->next;
				delete front;
				front = help;
			}
		}
	}

	template <typename T>
	void QueueList<T>::removeAll()
	{
		while (front)
		{
			dequeue();
		}

		front = nullptr;
		back = nullptr;
	}

	template <typename T>
	bool QueueList<T>::contains(T element) const
	{
		Node* help = front;
		while (help)
		{
			if (help->data == element)
				return true;
			help = help->next;
		}
		return false;
	}

	template <typename T>
	void QueueList<T>::remove(T element)
	{
		Node* help = front;
		Node* previous;
		while (help)
		{
			if (help->data == element)
			{
				if (help == front && front == back) // last element
				{
					front = nullptr;
					back = nullptr;
					delete help;
				}
				else if (help == front) // front element
				{
					front = front->next;
					delete help;
				}
				else if (help == back) // back element
				{
					previous->next = nullptr;
					back = previous;
					delete help;
				}
				else // middle element
				{
					previous->next = help->next;
					delete help;
				}

				break;
			}

			previous = help;
			help = help->next;
		}
	}

	template <typename T>
	QueueList<T>::QueueList()
	{
		front = nullptr;
		back = nullptr;
	}

	template <typename T>
	QueueList<T>::~QueueList()
	{
		removeAll();
	}


}}  // namespace fullsail_ai::fundamentals

#endif  // _FULLSAIL_AI_FUNDAMENTALS_QUEUELIST_TODO_H_

