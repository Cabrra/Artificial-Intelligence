using System;
using System.Drawing;
using FullSailAFI.GamePlaying.CoreAI;

//
// Modified: 2011/04/26, Derek Bliss (Full Sail University)
namespace FullSailAFI.GamePlaying
{
	/// <summary>
	/// Summary description for Options.
	/// </summary>
	public class Options
	{

		// Define the game options.
		public bool  ShowValidMoves;
		public bool  PreviewMoves;
		public bool  AnimateMoves;
		public Color BoardColor;
		public Color ValidMoveColor;
		public Color ActiveSquareColor;
		public Color MoveIndicatorColor;
		public bool  ComputerPlaysBlack;
		public bool  ComputerPlaysWhite;
		public int   BlackDifficulty;
        public int   WhiteDifficulty;
        public bool  BlackUsesFullSailAI;
        public bool  WhiteUsesFullSailAI;

		//
		// Creates a new Options object using the defaults.
		//
		public Options()
		{
			//
			// TODO: Add constructor logic here
			//

			// Initialize the game options to their default values.
			this.RestoreDefaults();
		}

		//
		// Creates a new Options object by copying an existing one.
		//
		public Options(Options options)
		{
			this.ShowValidMoves         = options.ShowValidMoves;
			this.PreviewMoves           = options.PreviewMoves;
			this.AnimateMoves           = options.AnimateMoves;
			this.BoardColor             = options.BoardColor;
			this.ValidMoveColor         = options.ValidMoveColor;
			this.ActiveSquareColor      = options.ActiveSquareColor;
			this.MoveIndicatorColor     = options.MoveIndicatorColor;
			this.ComputerPlaysBlack     = options.ComputerPlaysBlack;
			this.ComputerPlaysWhite     = options.ComputerPlaysWhite;
            this.BlackDifficulty        = options.BlackDifficulty;
            this.WhiteDifficulty        = options.WhiteDifficulty;
            this.BlackUsesFullSailAI    = options.BlackUsesFullSailAI;
            this.WhiteUsesFullSailAI    = options.WhiteUsesFullSailAI;
        }

		//
		// Restores all game options to their default values.
		//
		public void RestoreDefaults()
		{
			this.ShowValidMoves         = true;
			this.AnimateMoves           = true;
			this.PreviewMoves           = false;
			this.BoardColor             = SquareControl.NormalBackColorDefault;
			this.ValidMoveColor         = SquareControl.ValidMoveBackColorDefault;
			this.ActiveSquareColor      = SquareControl.ActiveSquareBackColorDefault;
			this.MoveIndicatorColor     = SquareControl.MoveIndicatorColorDefault;
			this.ComputerPlaysBlack     = false;
			this.ComputerPlaysWhite     = true;
            this.BlackDifficulty        = (int)Agent.Difficulty.Intermediate;
            this.WhiteDifficulty        = (int)Agent.Difficulty.Intermediate;
            this.BlackUsesFullSailAI    = true;
            this.WhiteUsesFullSailAI    = true;
        }
	}
}
