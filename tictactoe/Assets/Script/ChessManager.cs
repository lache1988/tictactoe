using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessManager : MonoBehaviour
{
    public enum GameState
    {
        setup,blueTurn,redTurn,blueWin,redWin,draw
    }

    public enum GameMode
    { 
        nomode,onePlayer,twoPlayer
    }
    //数据层board
    public int[,] boardlist = {
            { 0,0,0 },
            { 0,0,0 },
            { 0,0,0 }
    };

    //表现层grid
    public Grid grid00;
    public Grid grid01;
    public Grid grid02;
    public Grid grid10;
    public Grid grid11;
    public Grid grid12;
    public Grid grid20;
    public Grid grid21;
    public Grid grid22;

    public Grid[,] gridlist;


    public GameState _gameState;
    public bool isPlayerBlue = true;
    public GameMode _gamemode;
    private IEnumerator coroutine;
    // Start is called before the first frame update
    void Start()
    {
        //init
        _gameState = GameState.setup;
        gridlist = new[,] { { grid00, grid01, grid02 }, { grid10, grid11, grid12 }, { grid20, grid21, grid22 } };


        //棋盘数据重置
        BoardReset();
    }

    // Update is called once per frame
    void Update()
    {
        //同步格子数据到表现层
        UpdateGrid();
        //选择颜色期间跳过game
        if (_gameState == GameState.setup)
        {
            return;
        }

        //是否游戏结束
        if (MinimaxSolver.isMovesLeft(boardlist))
        {
            var score = MinimaxSolver.evaluate(boardlist);

            if (score == 10)
            {
                _gameState = GameState.blueWin;
                return;
            }
            else if (score == -10)
            {
                _gameState = GameState.redWin;
                return;
            }
        }
        else 
        {
            _gameState = GameState.draw;
            return;
        }

        
        if (_gamemode == GameMode.onePlayer)
        {
            //玩家vsAI
            PlayerVsAIMain();
        }
        else if (_gamemode == GameMode.twoPlayer)
        {
            //玩家vs玩家
            PlayerVsPlayerMain();
        }


    }
    public void BoardReset()
    {
        _gameState = GameState.blueTurn;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                boardlist[i, j] = 0;
                gridlist[i, j].Reset();
            }
    }

    void PlayerVsAIMain()
    {
        //蓝色回合，玩家为蓝色
        if (_gameState == GameState.blueTurn && isPlayerBlue)
        {
            //等待玩家输入
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    Grid grid = hit.collider.GetComponent<Grid>();
                    if (grid)
                    {
                        MakeMove(true, grid);
                    }

                }
            }
        }
        //蓝色回合，AI为蓝色
        else if (_gameState == GameState.blueTurn && !isPlayerBlue)
        {
            var coord = MinimaxSolver.MinimaxMain(boardlist, true);
            StartCoroutine(WaitAndMakeMove(1.5f, true, gridlist[coord[0], coord[1]]));

        }
        //红色回合，玩家为红色
        else if (_gameState == GameState.redTurn && !isPlayerBlue)
        {
            //等待玩家输入
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    Grid grid = hit.collider.GetComponent<Grid>();
                    if (grid)
                    {
                        MakeMove(false, grid);
                    }
                }
            }
        }
        //红色回合，AI为红色
        else if (_gameState == GameState.redTurn && isPlayerBlue)
        {
            var coord = MinimaxSolver.MinimaxMain(boardlist, false);
            StartCoroutine(WaitAndMakeMove(1.5f, false, gridlist[coord[0], coord[1]]));
        }
        
    }


    void PlayerVsPlayerMain()
    {
        //蓝色玩家回合
        if (_gameState == GameState.blueTurn)
        {
            //等待玩家输入
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                Grid grid = hit.collider.GetComponent<Grid>();
                    if (grid)
                    { 
                        MakeMove(true, grid);
                    }
                }
            }
        }
        //红色玩家回合
        else if (_gameState == GameState.redTurn)
        {
            //等待玩家输入
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    Grid grid = hit.collider.GetComponent<Grid>();
                    if (grid)
                    {
                        MakeMove(false, grid);
                    }
                }
            }
        }
    }
    //数据层向表现层同步
    void UpdateGrid()
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if (boardlist[i, j] == 1)
                    gridlist[i, j].SetState(true);

                if (boardlist[i, j] == 2)
                    gridlist[i, j].SetState(false);
            }
    }
    //操作向表现层和数据层更新，然后下一回合
    void MakeMove(bool isBlue, Grid grid)
    {
        if (grid._gridstate == Grid.GridState.IsEmpty)
        {
            boardlist[grid.gridX, grid.gridY] = isBlue ? 1 : 2;
            nextTurn();
        }
        else
        {
            Debug.Log("fail,this grid is occupied");
        }

    }
    void nextTurn()
    {
        if (_gameState == GameState.blueTurn)
        {
            _gameState = GameState.redTurn;
        }
        else if (_gameState == GameState.redTurn)
        {
            _gameState = GameState.blueTurn;
        }
    }

    public class MinimaxSolver
    {
        class Move
        {
            public int row, col;
        };

        static int blue = 1, red = 2;

        public static bool isMovesLeft(int[,] board)
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (board[i, j] == 0)
                        return true;
            return false;
        }

        public static int evaluate(int[,] b)
        {
            // Checking for Rows for X or O victory.
            for (int row = 0; row < 3; row++)
            {
                if (b[row, 0] == b[row, 1] &&
                    b[row, 1] == b[row, 2])
                {
                    if (b[row, 0] == blue)
                        return +10;
                    else if (b[row, 0] == red)
                        return -10;
                }
            }

            // Checking for Columns for X or O victory.
            for (int col = 0; col < 3; col++)
            {
                if (b[0, col] == b[1, col] &&
                    b[1, col] == b[2, col])
                {
                    if (b[0, col] == blue)
                        return +10;

                    else if (b[0, col] == red)
                        return -10;
                }
            }

            // Checking for Diagonals for X or O victory.
            if (b[0, 0] == b[1, 1] && b[1, 1] == b[2, 2])
            {
                if (b[0, 0] == blue)
                    return +10;
                else if (b[0, 0] == red)
                    return -10;
            }

            if (b[0, 2] == b[1, 1] && b[1, 1] == b[2, 0])
            {
                if (b[0, 2] == blue)
                    return +10;
                else if (b[0, 2] == red)
                    return -10;
            }

            // Else if none of them have won then return 0
            return 0;
        }

        static int minimax(int[,] board,int depth, bool isMax)
        {
            int score = evaluate(board);

            // If Maximizer has won the game
            // return his/her evaluated score
            if (score == 10)
                return score;

            // If Minimizer has won the game
            // return his/her evaluated score
            if (score == -10)
                return score;

            // If there are no more moves and
            // no winner then it is a tie
            if (isMovesLeft(board) == false)
                return 0;

            // If this maximizer's move
            if (isMax)
            {
                int best = -1000;

                // Traverse all cells
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty
                        if (board[i, j] == 0)
                        {
                            // Make the move
                            board[i, j] = blue;

                            // Call minimax recursively and choose
                            // the maximum value
                            best = Mathf.Max(best, minimax(board, depth +1, false));

                            // Undo the move
                            board[i, j] = 0;
                        }
                    }
                }
                return best;
            }

            // If this minimizer's move
            else
            {
                int best = 1000;

                // Traverse all cells
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty
                        if (board[i, j] == 0)
                        {
                            // Make the move
                            board[i, j] = red;

                            // Call minimax recursively and choose
                            // the minimum value
                            best = Mathf.Min(best, minimax(board, depth + 1, true));

                            // Undo the move
                            board[i, j] = 0;
                        }
                    }
                }
                return best;
            }
        }

        static Move findBestMove(int[,] board, bool isMax)
        {
            if (isMax)
            {
                int bestVal = -1000;
                Move bestMove = new Move();
                bestMove.row = -1;
                bestMove.col = -1;

                // Traverse all cells, evaluate minimax function
                // for all empty cells. And return the cell
                // with optimal value.
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty
                        if (board[i, j] == 0)
                        {
                            // Make the move
                            board[i, j] = blue;

                            // compute evaluation function for this
                            // move.
                            int moveVal = minimax(board, 0, false);

                            // Undo the move
                            board[i, j] = 0;

                            // If the value of the current move is
                            // more than the best value, then update
                            // best/
                            if (moveVal > bestVal)
                            {
                                bestMove.row = i;
                                bestMove.col = j;
                                bestVal = moveVal;
                            }
                        }
                    }
                }

                return bestMove;
            }
            else {
                int bestVal = 1000;
                Move bestMove = new Move();
                bestMove.row = -1;
                bestMove.col = -1;

                // Traverse all cells, evaluate minimax function
                // for all empty cells. And return the cell
                // with optimal value.
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        // Check if cell is empty
                        if (board[i, j] == 0)
                        {
                            // Make the move
                            board[i, j] = red;

                            // compute evaluation function for this
                            // move.
                            int moveVal = minimax(board, 0, true);

                            // Undo the move
                            board[i, j] = 0;

                            // If the value of the current move is
                            // more than the best value, then update
                            // best/
                            if (moveVal < bestVal)
                            {
                                bestMove.row = i;
                                bestMove.col = j;
                                bestVal = moveVal;
                            }
                        }
                    }
                }

                return bestMove;


            }
            

        }

        // Driver code
        public static int[] MinimaxMain(int[,] board, bool isMax)
        {

            Move bestMove = findBestMove(board,isMax);

            var rowcol = new int[] { bestMove.row, bestMove.col };
            return rowcol;
        }
    }

    private IEnumerator WaitAndMakeMove(float waitTime,bool isBlue,Grid grid)
    {
        yield return new WaitForSeconds(waitTime);
        MakeMove(isBlue, grid);
    }
}
