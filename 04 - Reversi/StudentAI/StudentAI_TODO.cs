using System;
using System.Collections.Generic;
using System.Text;
using FullSailAFI.GamePlaying.CoreAI;

namespace FullSailAFI.GamePlaying
{
    public class StudentAI : Behavior
    {
        public StudentAI() { }

        //
        // This function starts the look ahead process to find the best move
        // for the current player color.
        //
        public ComputerMove Run(int _color, Board _board, int _lookAheadDepth)
        {
            //TODO: the lab
           // throw new NotImplementedException();

            ComputerMove bestMove = null;
	        Board newState = new Board();
           List<ComputerMove> myMoves = new List<ComputerMove>();
	      
                //generate valid Moves for player;
           for (int i = 0; i < 8; i++)// row
           {
               for (int j = 0; j < 8; j++) //column
               {
                   ComputerMove tl = new ComputerMove(0, 0);
                   tl.row = i;
                   tl.col = j;
                   myMoves.Add(tl);
               }
           }

                 foreach(ComputerMove checkMoves in myMoves)
                 {
                     newState.Copy(_board);
                     if (newState.IsValidMove(_color, checkMoves.row, checkMoves.col))
                     {
		                 newState.MakeMove(_color, checkMoves.row, checkMoves.col);

                         if (newState.IsTerminalState() || _lookAheadDepth == 0)
                             checkMoves.rank = Evaluate(newState);
                          else
                             checkMoves.rank = Run(GetNextPlayer(_color, newState), newState, _lookAheadDepth - 1).rank;
                         //checkMoves.rank = Evaluate(newState);
                         if (_color == -1)
                         {
                             if (bestMove == null || checkMoves.rank < bestMove.rank)
                                 bestMove = checkMoves;
                         }
                         else
                         {
                             if (bestMove == null || checkMoves.rank > bestMove.rank)
                                 bestMove = checkMoves;
                         }
                     }
                 }
               
                // myMoves.Clear();
                 return bestMove;
        }

        #region Recommended Helper Functions

        private int Evaluate(Board _board)
        {
            //TODO: determine score based on position of pieces
            //throw new NotImplementedException();
            //add the values
            int value = 0;

            for (int i = 0; i < 8; i++)// row
            {
                for (int j = 0; j < 8; j++) //column
                {
                    int color = _board.GetSquareContents(i, j);
                    if ((i == 0 && j == 0) || (i == 7 && j == 0) || (i == 0 && j == 7) || (i == 7 && j == 7))
                        value += color * 100;
                    else if (i == 0 || i == 7 || j == 0 || j == 7)
                        value += color * 10;
                    else
                        value += color;
                }
            }

            if (_board.IsTerminalState())
            {
                if (value > 0)
                    value += 1000;
                else
                    value -= 1000;
            }

            return value;            
        }


        private int GetNextPlayer(int player, Board gameState)
	    {
	        if (gameState.HasAnyValidMove(-player))
	            return -player;
	        else
	            return player;
	    }


        #endregion

    }
}
