using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
    以下为数独具体实现，不依赖图形化
*/

//每一个小方格的信息
public class Cell
{
    //ID
    public Vector2Int ID = new Vector2Int();

    //数值
    public int value = 0;

    //所在行
    public int line = 0;

    //所在列
    public int column = 0;

    //所在宫
    public int grid = 0;

    //是否是初始生成数字
    public bool isOrigin = true;
}


public class SudokuManager
{
    //用这个作为范围限制
    const int Nine = 9;

    static int[] l_to_9 = new int[Nine] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    //小方格组成的二维数组
    public Cell[,] cell = new Cell[Nine, Nine];

    //每一行列宫对角线包含的格子的index
    static Vector2Int[,] lineIndex = new Vector2Int[Nine, Nine];
    static Vector2Int[,] columnIndex = new Vector2Int[Nine, Nine];
    static Vector2Int[,] gridIndex = new Vector2Int[Nine, Nine];
    static Vector2Int[,] diagIndex = new Vector2Int[2, Nine];

    /*
    *根据不同难度生成带有不同初始数字的数独
    *数独保证有解（挖空法）
    *待填数字分布较为平均，不挤在一起
    */
    public void GenerateSudoku()
    {
        //初始化数独行列宫的信息
        InitSudokuInfo();

        //DEBUG: 一个确定的数独
        //_Debug_SimpleSudoku();

        if (StaticValue.Get()._Difficult == StaticValue.Empty)
        {
            EmptySudoku();
            return;
        }

        //搜索出一个合法的解
        //DepthFirstSearch(0, 0);
        SolveSudoku();

        //挖空
        DeleteSomeCell();

    }

    //初始化数独行列宫的信息
    public void InitSudokuInfo()
    {
        //初始化每一个格子的信息
        //用_line,_column示范，之后可用i,j
        for (int _line = 0; _line < Nine; _line++)
        {
            for (int _column = 0; _column < Nine; _column++)
            {
                cell[_line, _column] = new Cell();
                cell[_line, _column].line = _line;
                cell[_line, _column].column = _column;
                cell[_line, _column].ID = new Vector2Int(_line, _column);
                cell[_line, _column].grid = (_line / 3) * 3 + (_column / 3);
            }
        }

        //初始化每一行列宫对角线包含的格子的index
        for (int i = 0; i < Nine; i++)
        {
            for (int j = 0; j < Nine; ++j)
            {
                lineIndex[i, j] = new Vector2Int(i, j);
                columnIndex[i, j] = new Vector2Int(j, i);
                gridIndex[i, j] = new Vector2Int((i / 3) * 3 + (j / 3), (i % 3) * 3 + (j % 3));
            }
            diagIndex[0, i] = new Vector2Int(i, i);
            diagIndex[1, i] = new Vector2Int(i, 8 - i);
        }

#if DEBUG_SUDOKU
        _Debug_SudokuInfo();
#endif

    }

    public void AddSudokuNumber(int i, int j, int num)
    {
        cell[i, j].value = num;
    }

    public void RemoveSudokuNumber(int i, int j)
    {
        cell[i, j].value = 0;
    }

    //检查填入的数独数字是不是在行列宫唯一，若冲突返回与其冲突的数字
    public Vector2Int CheckSudokunumber(int i, int j, int num)
    {
        int gridID = (i / 3) * 3 + j / 3;
        //行列宫
        for (int _pos = 0; _pos < Nine; ++_pos)
        {
            //这一行不能填这个数
            if ((lineIndex[i, _pos].x, lineIndex[i, _pos].y) != (i, j) &&
                cell[lineIndex[i, _pos].x, lineIndex[i, _pos].y].value == num)
            {

                return new Vector2Int(lineIndex[i, _pos].x, lineIndex[i, _pos].y);
            }
            //这一列不能填这个数
            if ((columnIndex[j, _pos].x, columnIndex[j, _pos].y) != (i, j) &&
                cell[columnIndex[j, _pos].x, columnIndex[j, _pos].y].value == num)
            {

                return new Vector2Int(columnIndex[j, _pos].x, columnIndex[j, _pos].y);
            }
            //这一宫不能填这个数
            if ((gridIndex[gridID, _pos].x, gridIndex[gridID, _pos].y) != (i, j) &&
                cell[gridIndex[gridID, _pos].x, gridIndex[gridID, _pos].y].value == num)
            {

                return new Vector2Int(gridIndex[gridID, _pos].x, gridIndex[gridID, _pos].y);
            }

            //对角线
            if (i == j)
            {
                if ((diagIndex[0, _pos].x, diagIndex[0, _pos].y) != (i, j) &&
                    cell[diagIndex[0, _pos].x, diagIndex[0, _pos].y].value == num)
                {
                    return new Vector2Int(diagIndex[0, _pos].x, diagIndex[0, _pos].y);
                }
            }
            if (i + j == 8)
            {
                if ((diagIndex[1, _pos].x, diagIndex[1, _pos].y) != (i, j) &&
                    cell[diagIndex[1, _pos].x, diagIndex[1, _pos].y].value == num)
                {
                    return new Vector2Int(diagIndex[1, _pos].x, diagIndex[1, _pos].y);
                }
            }
        }
        //返回(0,-1)表示合法。注意不能返回zero因为第一个格子就是zero
        return Vector2Int.down;
    }

    //检查填入的数独数字是不是在行列宫唯一
    public bool CanAdd(int i, int j, int num)
    {
        int gridID = (i / 3) * 3 + j / 3;
        //行列宫
        for (int _pos = 0; _pos < Nine; ++_pos)
        {
            //这一行不能填这个数
            if (
                cell[lineIndex[i, _pos].x, lineIndex[i, _pos].y].value == num)
            {

                return false;
            }
            //这一列不能填这个数
            if (
                cell[columnIndex[j, _pos].x, columnIndex[j, _pos].y].value == num)
            {

                return false;
            }
            //这一宫不能填这个数
            if (
                cell[gridIndex[gridID, _pos].x, gridIndex[gridID, _pos].y].value == num)
            {

                return false;
            }
            //对角线
            if (i == j)
            {
                if (
                    cell[diagIndex[0, _pos].x, diagIndex[0, _pos].y].value == num)
                {
                    return false;
                }
            }
            if (i + j == 8)
            {
                if (
                    cell[diagIndex[1, _pos].x, diagIndex[1, _pos].y].value == num)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //挖空，生成不同难度的数独局面
    private void DeleteSomeCell()
    {
        //根据难度确定需要去除的数的个数
        int delNumber = StaticValue.Get()._Difficult;

        //随机去除位置(挖洞)
        int[] hole = new int[Nine * Nine];
        float[] orders = new float[Nine * Nine];
        for (int i = 0; i < Nine * Nine; ++i)
        {
            hole[i] = i;
            orders[i] = Random.Range(0.0f, 1.0f);
        }
        System.Array.Sort(orders, hole);
        for (int i = 0; i < delNumber; ++i)
        {
            cell[hole[i] / 9, hole[i] % 9].value = 0;
            cell[hole[i] / 9, hole[i] % 9].isOrigin = false;
        }
    }

    //生成空数独
    private void EmptySudoku()
    {
        for (int i = 0; i < Nine; i++)
        {
            for (int j = 0; j < Nine; j++)
            {
                cell[i, j].value = 0;
                cell[i, j].isOrigin = false;
            }
        }
    }

    //检查是否完成
    public bool isFinish()
    {
        for (int i = 0; i < Nine; ++i)
        {
            for (int j = 0; j < Nine; ++j)
            {
                if (cell[i, j].value == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    List<int>[,] ValidNum = new List<int>[Nine, Nine];
    List<Vector2Int> ZeroIndex;

    //解决当前数独
    public bool SolveSudoku()
    {
        ZeroIndex = new List<Vector2Int>();
        //所有待填数字
        for (int i = 0; i < Nine; ++i)
        {
            for (int j = 0; j < Nine; ++j)
            {
                //随机化合法数字序列
                int[] nums = new int[Nine];
                float[] orders = new float[Nine];
                for(int _i=0;_i<Nine;++_i)
                {
                    nums[_i] = _i + 1;
                    orders[_i] = Random.Range(0.0f,1.0f);
                }
                System.Array.Sort(orders,nums);
                ValidNum[i,j] = new List<int>(nums);
                if (cell[i, j].value == 0)
                {
                    ZeroIndex.Add(new Vector2Int(i, j));
                }
            }
        }

        //排除所有不合法数字
        for (int i = 0; i < Nine; ++i)
        {
            for (int j = 0; j < Nine; ++j)
            {
                if (cell[i, j].value != 0)
                {
                    UpdateVaildNum(i, j);
                }
            }
        }

#if DEBUG_SUDOKU
        _Debug_ValidNum();
#endif

        if (AstarSearch())
        {
            return true;
        }

        return false;

    }

    //AstarSearch
    //估值为可填数字个数，每次选择可填数字最少的位置填数
    private bool AstarSearch()
    {
        if (ZeroIndex.Count == 0)
        {
            return true;
        }

        //找寻可填数字最少的位置
        Vector2Int pos = ZeroIndex[0];
        int minValidCount = ValidNum[pos.x, pos.y].Count;
        for (int i = 0; i < ZeroIndex.Count; ++i)
        {
            int tmp = ValidNum[ZeroIndex[i].x, ZeroIndex[i].y].Count;
            if (tmp == 0)
            {
                Debug.Log($"{ZeroIndex[i].x},{ZeroIndex[i].y} can't find a valid number!!");
                return false;
            }
            if (tmp < minValidCount)
            {
                pos = ZeroIndex[i];
                minValidCount = tmp;
            }
        }

        //保存目前状态的合法数字表
        List<int>[,] TempValid = new List<int>[Nine, Nine];
        DeepCopyValidNum(ValidNum, TempValid);

        //只有一个合法数字，直接填上
        if (minValidCount == 1)
        {
            cell[pos.x, pos.y].value = ValidNum[pos.x, pos.y][0];
            ZeroIndex.Remove(pos);
            UpdateVaildNum(pos.x, pos.y);

            Debug.Log($"{pos.x},{pos.y} got number {ValidNum[pos.x, pos.y][0]} !!!");

            if (AstarSearch())
            {
                return true;
            }

            cell[pos.x, pos.y].value = 0;
            ZeroIndex.Add(pos);
            DeepCopyValidNum(TempValid, ValidNum);
            return false;

        }

        //有两个及以上可能的合法数字，需要试填

        for (int _i = 0; _i < ValidNum[pos.x, pos.y].Count; ++_i)
        {
            Debug.Log($"{pos.x},{pos.y} trying {ValidNum[pos.x, pos.y][_i]} !!!");
            cell[pos.x, pos.y].value = ValidNum[pos.x, pos.y][_i];
            ZeroIndex.Remove(pos);
            UpdateVaildNum(pos.x, pos.y);
            if (AstarSearch())
            {
                return true;
            }
            cell[pos.x, pos.y].value = 0;
            ZeroIndex.Add(pos);
            DeepCopyValidNum(TempValid, ValidNum);
        }

        return false;

    }

    //深拷贝
    private void DeepCopyValidNum(List<int>[,] _arr, List<int>[,] _tar)
    {
        for (int i = 0; i < Nine; i++)
        {
            for (int j = 0; j < Nine; j++)
            {
                _tar[i, j] = new List<int>(_arr[i, j]);
            }
        }
    }

    public void UpdateVaildNum(int i, int j)
    {
        int num = cell[i, j].value;
        for (int _i = 0; _i < Nine; ++_i)
        {
            //行
            int lineX = lineIndex[i, _i].x;
            int lineY = lineIndex[i, _i].y;
            if ((lineX, lineY) != (i, j) && cell[lineX, lineY].value == 0)
            {
                for (int pos = 0; pos < ValidNum[lineX, lineY].Count; ++pos)
                {
                    if (ValidNum[lineX, lineY][pos] == num)
                    {
                        ValidNum[lineX, lineY].RemoveAt(pos);
                        break;
                    }
                }
            }

            //列
            int columnX = columnIndex[j, _i].x;
            int columnY = columnIndex[j, _i].y;
            if ((columnX, columnY) != (i, j) && cell[columnX, columnY].value == 0)
            {
                for (int pos = 0; pos < ValidNum[columnX, columnY].Count; ++pos)
                {
                    if (ValidNum[columnX, columnY][pos] == num)
                    {
                        ValidNum[columnX, columnY].RemoveAt(pos);
                        break;
                    }
                }
            }

            //宫
            int gridID = cell[i, j].grid;
            int gridX = gridIndex[gridID, _i].x;
            int gridY = gridIndex[gridID, _i].y;
            if ((gridX, gridY) != (i, j) && cell[gridX, gridY].value == 0)
            {
                for (int pos = 0; pos < ValidNum[gridX, gridY].Count; ++pos)
                {
                    if (ValidNum[gridX, gridY][pos] == num)
                    {
                        ValidNum[gridX, gridY].RemoveAt(pos);
                        break;
                    }
                }
            }

            //对角线
            if (i == j)
            {
                int diagID = 0;
                int diagX = diagIndex[diagID, _i].x;
                int diagY = diagIndex[diagID, _i].y;
                if ((diagX, diagY) != (i, j) && cell[diagX, diagY].value == 0)
                {
                    for (int pos = 0; pos < ValidNum[diagX, diagY].Count; ++pos)
                    {
                        if (ValidNum[diagX, diagY][pos] == num)
                        {
                            ValidNum[diagX, diagY].RemoveAt(pos);
                            break;
                        }
                    }
                }
            }
            if (i + j == 8)
            {
                int diagID = 1;
                int diagX = diagIndex[diagID, _i].x;
                int diagY = diagIndex[diagID, _i].y;
                if ((diagX, diagY) != (i, j) && cell[diagX, diagY].value == 0)
                {
                    for (int pos = 0; pos < ValidNum[diagX, diagY].Count; ++pos)
                    {
                        if (ValidNum[diagX, diagY][pos] == num)
                        {
                            ValidNum[diagX, diagY].RemoveAt(pos);
                            break;
                        }
                    }
                }
            }

        }
    }

    private int HintSearch(int i, int j)
    {
        if (i == Nine)
        {
            return 1;   //Successful!
        }

        //这个位置已经有数了
        if (cell[i, j].value != 0)
        {
            if (HintSearch(i + (j + 1) / 9, (j + 1) % 9) == 1)
            {
                //搜索成功，返回1
                return 1;
            }
            else
            {
                return 0;
            }
        }

        //开始搜索！
        for (int _i = 1; _i <= Nine; _i++)
        {
            //可以填这个数
            if (CanAdd(i, j, _i))
            {
                cell[i, j].value = _i;

                //是否搜索成功
                if (HintSearch(i + (j + 1) / 9, (j + 1) % 9) == 1)
                {
                    cell[i, j].value = 0;
                    //搜索成功，返回1
                    return 1;
                }
                cell[i, j].value = 0;
            }
        }
        //搜索失败，返回0
        return 0;
    }

    //获得一个提示数字
    public int GetHintNumber(int i, int j)
    {
        if (cell[i, j].value != 0)
        {
            Debug.Log("Try to get hint at a position which already have number");
            return 0;
        }

        //从1到9搜索
        for (int num = 1; num <= 9; ++num)
        {
            if (CanAdd(i, j, num))
            {
                cell[i, j].value = num;

                if (HintSearch(0, 0) == 1)
                {
                    return num;
                }

                cell[i, j].value = 0;
            }
        }
        return 0;
    }

    //debug用，一个测试数独
    private void _Debug_SimpleSudoku()
    {
        int[,] Arr;
        // Arr = new int[Nine, Nine]
        // {
        //     {4,1,5,2,6,8,7,9,3},
        //     {3,2,9,7,4,1,5,6,8},
        //     {6,7,8,3,5,9,2,1,4},
        //     {9,3,2,5,7,4,1,8,6},
        //     {8,5,4,9,1,6,3,7,2},
        //     {1,6,7,8,3,2,4,5,9},
        //     {5,8,3,6,2,7,9,4,1},
        //     {2,9,1,4,8,5,6,3,7},
        //     {7,4,6,1,9,3,8,2,5}
        // };
        Arr = new int[Nine, Nine]
        {
            {4,1,0,0,0,0,0,9,3},
            {3,2,9,0,0,0,5,0,8},
            {6,0,8,0,5,9,2,0,4},
            {0,0,0,0,7,4,1,0,6},
            {8,0,0,0,1,0,0,0,2},
            {0,0,0,0,0,0,0,0,0},
            {5,8,3,6,2,7,9,4,1},
            {2,9,1,4,8,5,6,3,7},
            {7,4,6,1,9,3,8,2,5}
        };
        for (int i = 0; i < Nine; i++)
        {
            for (int j = 0; j < Nine; j++)
            {
                cell[i, j].value = Arr[i, j];
                cell[i, j].isOrigin = (Arr[i, j] == 0 ? false : true);
            }
        }
    }

    //Debug用，检查行列宫的index是不是正确
    private void _Debug_SudokuInfo()
    {

        for (int i = 0; i < Nine; ++i)
        {
            for (int j = 0; j < Nine; ++j)
            {
                if (cell[lineIndex[i, j].x, lineIndex[i, j].y].line != i)
                {
                    Debug.Log($"lineIndex {i},{j} Error!!!");
                }
                if (cell[columnIndex[i, j].x, columnIndex[i, j].y].column != i)
                {
                    Debug.Log($"columnIndex {i},{j} Error!!!");
                }
                if (cell[gridIndex[i, j].x, gridIndex[i, j].y].grid != i)
                {
                    Debug.Log($"gridIndex {i},{j} Error!!!");
                }
            }
        }
    }

    //Debug用，检查validNum是不是确实合法
    private void _Debug_ValidNum()
    {
        for (int i = 0; i < Nine; ++i)
        {
            for (int j = 0; j < Nine; ++j)
            {
                if (cell[i, j].value == 0)
                {

                    string str = string.Empty;
                    for (int _p = 0; _p < ValidNum[i, j].Count; ++_p)
                    {
                        str = str + ValidNum[i, j][_p].ToString() + " ";
                    }
                    Debug.Log($"ValidNum {i} {j} : " + str);
                }
            }
        }
    }

}
