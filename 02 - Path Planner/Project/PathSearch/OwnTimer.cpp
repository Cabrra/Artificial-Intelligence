#include "OwnTimer.h"

OwnTimer::OwnTimer()
{
	resetted = true;
	running = false;
	beg = 0;
	end = 0;
}


void OwnTimer::start()
{
	if (!running) {
		if (resetted)
			beg = (long)clock();
		else
			beg -= end - (long)clock();
		running = true;
		resetted = false;
	}
}


void OwnTimer::stop()
{
	if (running) 
	{
		end = (long)clock();
		running = false;
	}
}


void OwnTimer::reset() {
	bool wereRunning = running;
	if (wereRunning)
		stop();
	resetted = true;
	beg = 0;
	end = 0;
	if (wereRunning)
		start();
}


bool OwnTimer::isRunning() {
	return running;
}


long OwnTimer::getTime() {
	if (running)
		return ((long)clock() - beg) / CLOCKS_PER_SEC;
	else
		return end - beg;
}


bool OwnTimer::isOver(long seconds) {
	return seconds >= getTime();
}