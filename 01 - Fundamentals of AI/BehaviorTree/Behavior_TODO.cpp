//! \file Behavior_TODO.cpp
//! \brief Implements the <code>fullsail_ai::fundamentals::Behavior</code> abstract class.
//! \author Jeremiah Blanchard with code from Cromwell D. Enage

// #include <queue>   NOTE: Only use if your QueueList is not working
#include <stack>
#include "Behavior.h"
#include"../QueueList/QueueList.h"

namespace fullsail_ai { namespace fundamentals {

	unsigned Behavior::createdCount = 0;
	unsigned Behavior::destroyedCount = 0;

	unsigned Behavior::getCreatedCount()
	{
		return createdCount;
	}

	unsigned Behavior::getDestroyedCount()
	{
		return destroyedCount;
	}

	Behavior::Behavior(char const* _description)
	{
		description = _description;
	}

	char const* Behavior::toString() const
	{
		return description;
	}

	// DO NOT Change this function, it is supposed to just return false
	bool Behavior::isLeaf() const
	{
		return false;
	}

	//! \todo
	//!   - Implement the <code>Behavior</code> methods.

	size_t Behavior::getChildCount() const
	{
		return children.size();
	}

	Behavior* Behavior::getChild(size_t i)
	{
		if (i < children.size())
			return children[i];
	}
	
	Behavior const* Behavior::getChild(size_t i) const
	{
		if (i < children.size())
			return children[i];
	}

	void Behavior::addChild(Behavior* child)
	{
		child->parent = this;
		children.push_back(child);
	}

	void Behavior::breadthFirstTraverse(void(*dataFunction)(Behavior const*)) const
	{
		QueueList<const Behavior*> queue;
		queue.enqueue(this); 
		const Behavior* node;
		while (!queue.isEmpty())
		{
			node = queue.getFront();
			size_t mySiz = node->getChildCount();
			queue.dequeue();
			dataFunction(node);
			for (size_t i = 0; i < mySiz; i++)
				queue.enqueue(node->children[i]);
		}
	}

	void Behavior::preOrderTraverse(void(*dataFunction)(Behavior const*)) const
	{
		size_t mySiz = getChildCount();
		dataFunction(this);
		for (size_t i = 0; i < mySiz; i++)
		{
			children[i]->preOrderTraverse(dataFunction);
		}
	}

	void Behavior::postOrderTraverse(void(*dataFunction)(Behavior const*)) const
	{
		size_t mySiz = getChildCount();
		for (size_t i = 0; i < mySiz; i++)
		{
			children[i]->postOrderTraverse(dataFunction);
		}
		dataFunction(this);
	}

}}  // namespace fullsail_ai::fundamentals
