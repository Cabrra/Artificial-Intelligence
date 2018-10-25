#include "PathSearch.h"

//#include <vld.h>

namespace fullsail_ai { namespace algorithms {

	PathSearch::PathSearch() : myPriority(IsMyGreater)
	{
		myTimer.start();
		goalTile = nullptr;
		firstTile = nullptr;
		hWeight = 1.2f;
	}

	PathSearch::~PathSearch()
	{
		myTimer.stop();

		if (goalTile != nullptr)
			delete goalTile;
		goalTile = nullptr;

		if (firstTile != nullptr)
			delete firstTile;
		firstTile = nullptr;
	}

	double PathSearch::Estimate(pair<double, double> start, pair<double, double> goal)
	{
		double x, y;
		x = start.first - goal.first;
		y = start.second - goal.second;
		return (sqrt(x * x + y * y));
	}

	void PathSearch::initialize(TileMap* _tileMap)
	{
		myTileMapCopy = _tileMap;
		int rows = _tileMap->getColumnCount();
		int columns = _tileMap->getRowCount();
		for (int y = 0; y < rows; y++)
		{
			for (int x = 0; x < columns; x++) // check raw data
			{
				Tile* myTil = _tileMap->getTile(x, y);
				if (myTil->getWeight() != 0)
				{
					//create search node
					SearchNode* newNode = new SearchNode;
					newNode->data = myTil;
					//pathNodes.push_back(newNode);
					pair<int, int> myPair;
					myPair.first = newNode->data->getRow();
					myPair.second= newNode->data->getColumn();
					pathNodes[myPair] = newNode;
				}
			}
		}
		//size_t nodesSize = pathNodes.size();
		//for (size_t x = 0; x < nodesSize; x++)

		//set neighbors
		for (auto it = pathNodes.begin(); it != pathNodes.end(); ++it)
		{
			SearchNode* current = it->second;
			//always matches
			Tile* findTile;
			auto finder = pathNodes.begin();
			pair<int, int> filter;

			//LEFT
			findTile = _tileMap->getTile(current->data->getRow(), current->data->getColumn() - 1);
			if (findTile != nullptr && findTile->getWeight() != 0)
			{
				filter.first = findTile->getRow();
				filter.second= findTile->getColumn();

				finder = pathNodes.find(filter);
				if (finder->second != nullptr)
				{
					Edge* left = new Edge;
					left->endpoint = finder->second;
					left->cost = left->endpoint->data->getWeight();

					if (left->endpoint->data->getColumn() == (current->data->getColumn() - 1))
						current->edges.push_back(left);
					else
						delete left;
				}
			}
			//RIGHT
			findTile = _tileMap->getTile(current->data->getRow(), current->data->getColumn() + 1);
			if (findTile != nullptr && findTile->getWeight() != 0)
			{
				filter.first = findTile->getRow();
				filter.second = findTile->getColumn();

				finder = pathNodes.find(filter);
				if (finder->second != nullptr)
				{
					Edge* right = new Edge;
					right->endpoint = finder->second;
					right->cost = right->endpoint->data->getWeight();
					if (right->endpoint->data->getColumn() == (current->data->getColumn() + 1))
						current->edges.push_back(right);
					else
						delete right;
				}
			}

			//UP
			findTile = _tileMap->getTile(current->data->getRow() - 1, current->data->getColumn());
			if (findTile != nullptr && findTile->getWeight() != 0)
			{
				filter.first = findTile->getRow();
				filter.second = findTile->getColumn();

				finder = pathNodes.find(filter);
				if (finder->second != nullptr)
				{
					Edge* up = new Edge;
					up->endpoint = finder->second;
					up->cost = up->endpoint->data->getWeight();
					if (up->endpoint->data->getRow() == (current->data->getRow() - 1))
						current->edges.push_back(up);
					else
						delete up;
				}
			}
			//DOWN
			findTile = _tileMap->getTile(current->data->getRow() + 1, current->data->getColumn());
			if (findTile != nullptr && findTile->getWeight() != 0)
			{
				filter.first = findTile->getRow();
				filter.second = findTile->getColumn();

				finder = pathNodes.find(filter);
				if (finder->second != nullptr)
				{
					Edge* down = new Edge;
					down->endpoint = finder->second;
					down->cost = down->endpoint->data->getWeight();
					if (down->endpoint->data->getRow() == (current->data->getRow() + 1))
						current->edges.push_back(down);
					else
						delete down;
				}
			}

			//SPECIALS
			if (current->data->getRow() % 2 == 0) //Even Row
			{
				//UPLEFT
				findTile = _tileMap->getTile(current->data->getRow() - 1, current->data->getColumn() - 1);
				if (findTile != nullptr && findTile->getWeight() != 0)
				{
					filter.first = findTile->getRow();
					filter.second = findTile->getColumn();

					finder = pathNodes.find(filter);
					if (finder->second != nullptr)
					{
						Edge* upLeft = new Edge;
						upLeft->endpoint = finder->second;
						upLeft->cost = upLeft->endpoint->data->getWeight();
						if (upLeft->endpoint->data->getColumn() == (current->data->getColumn() - 1) && upLeft->endpoint->data->getRow() == (current->data->getRow() - 1))
							current->edges.push_back(upLeft);
						else
							delete upLeft;
					}
				}
		
				//DOWNLEFT
				findTile = _tileMap->getTile(current->data->getRow() + 1, current->data->getColumn() - 1);
				if (findTile != nullptr && findTile->getWeight() != 0)
				{
					filter.first = findTile->getRow();
					filter.second = findTile->getColumn();

					finder = pathNodes.find(filter);
					if (finder->second != nullptr)
					{
						Edge* downLeft = new Edge;
						downLeft->endpoint = finder->second;
						downLeft->cost = downLeft->endpoint->data->getWeight();
						if (downLeft->endpoint->data->getRow() == (current->data->getRow() + 1) && downLeft->endpoint->data->getColumn() == (current->data->getColumn() - 1))
							current->edges.push_back(downLeft);
						else
							delete downLeft;
					}
				}
			}
			else //odd row
			{
				//UPRIGHT
				findTile = _tileMap->getTile(current->data->getRow() - 1, current->data->getColumn() + 1);
				if (findTile != nullptr && findTile->getWeight() != 0)
				{
					filter.first = findTile->getRow();
					filter.second = findTile->getColumn();

					finder = pathNodes.find(filter);
					if (finder->second != nullptr)
					{
						Edge* upRight = new Edge;
						upRight->endpoint = finder->second;
						upRight->cost = upRight->endpoint->data->getWeight();
						if (upRight->endpoint->data->getRow() == (current->data->getRow() - 1) && upRight->endpoint->data->getColumn() == (current->data->getColumn() + 1))
							current->edges.push_back(upRight);
						else
							delete upRight;
					}
				}

				//DOWNRIGHT
				findTile = _tileMap->getTile(current->data->getRow() + 1, current->data->getColumn() + 1);
				if (findTile != nullptr && findTile->getWeight() != 0)
				{
					filter.first = findTile->getRow();
					filter.second = findTile->getColumn();

					finder = pathNodes.find(filter);
					if (finder->second != nullptr)
					{
						Edge* downRight = new Edge;
						downRight->endpoint = finder->second;
						downRight->cost = downRight->endpoint->data->getWeight();
						if (downRight->endpoint->data->getRow() == (current->data->getRow() + 1) && downRight->endpoint->data->getColumn() == (current->data->getColumn() + 1))
							current->edges.push_back(downRight);
						else
							delete downRight;
					}
				}
			}
		}
	}

	void PathSearch::enter(int startRow, int startColumn, int goalRow, int goalColumn)
	{
		pair<int, int> filter;
		//goal node
		filter.first = goalRow;
		filter.second = goalColumn;
		auto finder = pathNodes.find(filter);
		if (goalTile != nullptr)
		{
			delete goalTile;
			goalTile = nullptr;
		}

		goalTile = new SearchNode;
		goalTile->data = finder->second->data;
		
		//start node
		filter.first = startRow;
		filter.second = startColumn;
		finder = pathNodes.find(filter);

		if (firstTile != nullptr)
		{
			delete firstTile;
			firstTile = nullptr;
		}
		firstTile = new SearchNode;
		firstTile->data = finder->second->data;
		PlannerNode* firstNode = new PlannerNode;
		firstNode->SetVertex(finder->second);
		firstNode->SetParent(nullptr);
		myPriority.push(firstNode);
		//add to visited

		pair<double, double>  estimaS;
		estimaS.first = firstNode->GetVertex()->data->getXCoordinate();
		estimaS.second = firstNode->GetVertex()->data->getYCoordinate();
		goalHeuristic.first = goalTile->data->getXCoordinate();
		goalHeuristic.second = goalTile->data->getYCoordinate();
		double myHeu = Estimate(estimaS, goalHeuristic);
		firstNode->SetHeuristicCost(myHeu);
		firstNode->SetGiven(0);
		firstNode->SetFinalCost(firstNode->GetHeuristicCost() * hWeight);

		visited[finder->first] = firstNode;
		isFinish = false;
	}

	void PathSearch::update(long timeslice)
	{
		long start = myTimer.getTime();
		while (!myPriority.empty())
		{
			PlannerNode* current = myPriority.front();
			//current->GetVertex()->data->setFill(0xFF0000FF);
			if (current->GetVertex()->data->getColumn() == goalTile->data->getColumn() && current->GetVertex()->data->getRow() == goalTile->data->getRow())
			{
				isFinish = true;
				break;
			}

			myPriority.pop();
			//current->GetVertex()->data->setOutline(0xFFFFFFFF);
			rsize_t mySiz = current->GetVertex()->edges.size();

			for (size_t i = 0; i < mySiz; i++)
			{
				SearchNode* successor = current->GetVertex()->edges[i]->endpoint;
				
				double tempGiven = current->GetGiven() + (successor->data->getWeight() * myTileMapCopy->getTileRadius() * 2);

				pair<int, int> aux;
				aux.first = successor->data->getRow();
				aux.second = successor->data->getColumn();
				if (visited[aux] != NULL)
				{
					if (tempGiven < visited[aux]->GetGiven())
					{
						PlannerNode* successorNode = visited[aux];
						myPriority.remove(successorNode);
						successorNode->SetGiven(tempGiven);
						//successorNode->GetVertex()->data->setFill(successorNode->GetVertex()->data->getFill() - 20);
						successorNode->SetFinalCost(tempGiven + successorNode->GetHeuristicCost() * hWeight);
						successorNode->SetParent(current);
						myPriority.push(successorNode);
					}
				}
				else
				{
					PlannerNode* successorNode = new PlannerNode(successor);
					//successorNode->GetVertex()->data->setFill(0xFF000080);
					successorNode->SetParent(current);
					successorNode->SetGiven(tempGiven);

					pair<double, double>  estimaS;
					estimaS.first = successor->data->getXCoordinate();
					estimaS.second = successor->data->getYCoordinate();
					double myHeu = Estimate(estimaS, goalHeuristic);

					successorNode->SetHeuristicCost(myHeu);
					successorNode->SetFinalCost(tempGiven + myHeu * hWeight);

					visited[aux] = successorNode;
					myPriority.push(successorNode);

				}
			}

			long segment = myTimer.getTime();
			timeslice -= (segment - start);
			if (timeslice <= 0)
				break;
			start = myTimer.getTime();
		}
	}

	void PathSearch::exit()
	{
		if (!visited.empty())
		{
			auto it = visited.begin();
			while (it != visited.end())
			{
				delete (it->second);
				++it;
			}
		}

		visited.clear();
		myPriority.clear();
	}

	void PathSearch::shutdown()
	{
		if (!pathNodes.empty())
		{
			auto it = pathNodes.begin();
			while (it != pathNodes.end())
			{
				size_t edgeSize = it->second->edges.size();
				for (rsize_t i= 0; i < edgeSize; i++)
				{
					Edge* aux = it->second->edges.back();
					it->second->edges.pop_back();
					delete aux;
				}
				delete (it->second);
				++it;
			}
			pathNodes.clear();
		}

		if (!visited.empty())
		{
			auto it = visited.begin();
			while (it != visited.end())
			{
				delete (it->second);
				++it;
			}
			visited.clear();
		}


		myPriority.clear();
	}

	bool PathSearch::isDone() const
	{
		if (isFinish == true)
			return true;
		else
			return false;
	}

	std::vector<Tile const*> const PathSearch::getSolution() const
	{
		std::vector<Tile const*> temp;
		PlannerNode* myFirstTile = myPriority.front();
		//cout << "Given cost: " << myFirstTile->GetGiven() << "\n";
		//cout << "Heuristic cost: " << myFirstTile->GetHeuristicCost() << "\n";
		//cout << "final cost: " << myFirstTile->GetFinalCost() << "\n";
		while (myFirstTile != nullptr)
		{
			temp.push_back(myFirstTile->GetVertex()->data);
			myFirstTile = myFirstTile->GetParent();
		}
		
		return temp;
	}
}}  // namespace fullsail_ai::algorithms

