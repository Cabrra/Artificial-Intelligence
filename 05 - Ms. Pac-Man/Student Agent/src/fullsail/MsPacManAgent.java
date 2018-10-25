package fullsail;

import java.util.ArrayList;
import game.controllers.PacManController;
import game.core.G;
import game.core.Game;

public class MsPacManAgent implements PacManController// , Constants
{
	private int followDistance =  66;
	private int pillDistance = 20;
	
	private int pinkRunDistance = 5;
	private int redRunDistance = 4;
	private int blueRunDistance = 5;
	private int orangeRunDistance = 5;
	
	private int pinkDangerDistance = 12;
	private int redDangerDistance = 14;
	private int blueDangerDistance = 6;
	private int orangeDangerDistance = 7;
	
	//private int DangerDistance = ;

	public int getAction(Game game, long time)
	{
		int eval = EvaluateRun(game);
		if (eval == 1)
			return AvoidGhost(game);
		else if (eval == -1)
			return FollowGhost(game);
		else
			return FollowPill(game);
	}

	private int EvaluateRun(Game game) // check this
	{
		int pacman = game.getCurPacManLoc();
		int[] ghostDistances = new int[Game.NUM_GHOSTS];
		int[] ghostPos = new int[Game.NUM_GHOSTS];

		int result = 0;

		for (int i = 0; i < Game.NUM_GHOSTS; i++)
		{
			ghostPos[i] = game.getCurGhostLoc(i);
			ghostDistances[i] = (int) game.getEuclideanDistance(ghostPos[i], pacman);
																					 
			if (game.isEdible(i) && ghostDistances[i] < followDistance)
				result = -1; // GO FOR IT
			if (ghostPos[i] != game.getInitialGhostsPosition() && !game.isEdible(i))
			{
				if (i == 0 && ghostDistances[i] < redRunDistance ) //red
				{
					result = 1; // RUN
					break;
				}
				else if (i == 1 && ghostDistances[i] < pinkRunDistance ) //pink
				{
					result = 1; // RUN
					break;
				}
				else if (i == 2 && ghostDistances[i] < orangeRunDistance ) //orange
				{
					result = 1; // RUN
					break;
				}
				else if (i == 3 && ghostDistances[i] < blueRunDistance ) //cyan
				{
					result = 1; // RUN
					break;
				}
			}
		}
		return result;
	}

	private int FollowPill(Game game)
	{
		// state 1 - follow pills
		int[] myPills = game.getPillIndices();
		int[] myPower = game.getPowerPillIndices();
		int pacman = game.getCurPacManLoc();

		ArrayList<Integer> myVector = new ArrayList<Integer>();

		for (int i = 0; i < myPills.length; i++)
		{
			if (game.checkPill(i))
				myVector.add(myPills[i]);
		}

		for (int i = 0; i < myPower.length; i++)
		{
			if (game.checkPowerPill(i))
				myVector.add(myPower[i]);
		}

		int[] converted = new int[myVector.size()];
		for (int i = 0; i < converted.length; i++)
		{
			converted[i] = myVector.get(i);
		}

		int target = game.getTarget(pacman, converted, true, G.DM.MANHATTEN);

		int pill = game.getNextPacManDir(target, true, Game.DM.PATH);

		// wait in powerpill
		if ((game.getPowerPillIndex(target) != -1 && game.checkPowerPill(game.getPowerPillIndex(target))) || (game.isJunction(pacman) && game.getEuclideanDistance(pacman, target) < pillDistance ))
		{
			if (game.getEuclideanDistance(pacman, target) < 2)
				pill = game.getReverse(pill);
		}

		return pill;
	}

	private int AvoidGhost(Game game)
	{
		int pacman = game.getCurPacManLoc();
		int[] ghostPos = new int[Game.NUM_GHOSTS];
		// int ghostIndex = -1;

		for (int i = 0; i < Game.NUM_GHOSTS; i++)
		{
			ghostPos[i] = game.getCurGhostLoc(i);
		}

		int ghostTarget = game.getTarget(pacman, ghostPos, true, G.DM.MANHATTEN);

		int result;
		int runDistance = 0;
		
		for (int i = 0; i < Game.NUM_GHOSTS; i++)
		{
			if (ghostPos[i] == ghostTarget ) 
			{
				if (i == 0 )//red
					runDistance = redRunDistance;
				else if (i == 1 ) //pink
					runDistance = pinkRunDistance;
				else if (i == 2 ) //orange
					runDistance = orangeRunDistance;
				else if (i == 3) //cyan
					runDistance = blueRunDistance;
			}
		}
		
		if (game.getEuclideanDistance(ghostTarget, pacman) < runDistance)
			result = game.getNextPacManDir(ghostTarget, false, Game.DM.EUCLID);
		else
			result = FollowPill(game);

		// if it is near a powerpill
		int[] myPower = game.getPowerPillIndices();

		ArrayList<Integer> myVector = new ArrayList<Integer>();
		for (int i = 0; i < myPower.length; i++)
		{
			if (game.checkPowerPill(i))
				myVector.add(myPower[i]);
		}
		int[] converted = new int[myVector.size()];
		for (int i = 0; i < converted.length; i++)
		{
			converted[i] = myVector.get(i);
		}
		int target = game.getTarget(pacman, converted, true, G.DM.PATH);
		if (target != -1 && game.getPathDistance(pacman, target) < 2)
			result = game.getNextPacManDir(target, true, Game.DM.PATH);
		return result;
	}

	private int FollowGhost(Game game)
	{
		int pacman = game.getCurPacManLoc();
		int[] ghostPos = new int[Game.NUM_GHOSTS];

		int result = -1;

		for (int i = 0; i < Game.NUM_GHOSTS; i++)
		{
			ghostPos[i] = game.getCurGhostLoc(i);
		}
		int ghostTarget = -1;
		int DangerDistance = 0;
		for (int i = 0; i < Game.NUM_GHOSTS; i++)
		{
			if (i == 0 )//red
				DangerDistance = redDangerDistance;
			else if (i == 1 ) //pink
				DangerDistance = pinkDangerDistance;
			else if (i == 2 ) //orange
				DangerDistance = orangeDangerDistance;
			else if (i == 3) //cyan
				DangerDistance = blueDangerDistance;
			
			if (game.isEdible(i) && game.getEuclideanDistance(ghostPos[i], pacman) < followDistance)
			{
				ghostTarget = ghostPos[i];
				break;
			}
			else if (!game.isEdible(i) && game.getManhattenDistance(ghostPos[i], pacman) < DangerDistance) 
			{
				return AvoidGhost(game);
			}
		}

		if (ghostTarget != -1)
			result = game.getNextPacManDir(ghostTarget, true, Game.DM.PATH);
		else
			result = FollowPill(game);

		return result;
	}

}