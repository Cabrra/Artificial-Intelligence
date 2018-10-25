#include <iostream>
#include <conio.h>
#include <time.h>	

class OwnTimer 
{
public:
	OwnTimer();
	void start();
	void stop();
	void reset();
	bool isRunning();
	long getTime();
	bool isOver(long seconds);
private:
	bool resetted;
	bool running;
	long  beg;
	long  end;
};