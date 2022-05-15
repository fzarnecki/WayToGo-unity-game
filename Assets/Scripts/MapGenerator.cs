using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;


public class MapGenerator
{
    private const int SEED = 69;

    public static List<Cube> GeneratePathForLevel(int size, int minL, int maxL, int level, Cube[] allCubes) {
        
        Map map = new Map(SEED, size, minL, maxL);
        // seed is set in GeneratePath method, but set at the beginning as well in case some random value will be needed before that
        // Level will be generated based on level number, size of the map and length of the path generated

        map.GeneratePath(level);

        // Setting & returning the generated path
        List<Cube> path = new List<Cube>();

        for (int i = 0; i < map.GetCurrentLength(); i++)
        {
            path.Add(GetCubeAt(map.GetFirstCord(i), map.GetSecondCord(i), size, allCubes));
        }

        return path;
    }

    private static Cube GetCubeAt(int i, int j, int size, Cube[] allCubes) {
        return allCubes[i * size + j];
    }

}


public class Map
{
    public Map(int seed, int size, int lMin, int lMax) {
        SetRandom(seed);
        SetSize(size);
        lengthMin = lMin;
        lengthMax = lMax;
    }

    public const int MAP_SIZE_MIN = 5;
    public const int MAP_SIZE_MAX = 8;
    public const int LEVELS_TO_CHANGE_LENGTH = 3;

    const char empty = '-';
    private int pathSign = -1;

    private void ResetPathSign() { pathSign = -1; }

    private char[][] map;
    private int size;

    System.Random random;

    int lengthMin;
    int lengthMax;
    int currentLength;

    // Arrays keeping track of the coordinates of path points
    int[] cordi;
    int[] cordj;

    private void ResetHelperCords(int l) {
        cordi = new int[l];
        cordj = new int[l];
    }

    enum directions
    {
        left,
        right,
        up,
        down,
        none
    }

    struct pos
    {
        public int x;
        public int y;
    }

    /***/

    public void SetRandom(int seed) {
        random = new System.Random(seed);
    }

    public void InitializeMap(int l) {
        ResetPathSign();
        ResetHelperCords(l);
        map = new char[GetSize()][];

        for (int i = 0; i < GetSize(); i++)
        {
            map[i] = new char[GetSize()];
            for (int j = 0; j < GetSize(); j++)
            {
                map[i][j] = new char();
                map[i][j] = empty;
            }
        }
    }

    public void GeneratePath(int levelNr) {
        int length;
        int size = GetSize();
        if (size == MAP_SIZE_MIN)
            length = lengthMin + ((int)(levelNr / LEVELS_TO_CHANGE_LENGTH) * 1);
        else
            length = lengthMin + ((int)(levelNr / LEVELS_TO_CHANGE_LENGTH) * (size - MAP_SIZE_MIN));    // length grows every 5 levels, until reaches max

        if (length >= lengthMax)
            length = random.Next(lengthMax - size, lengthMax);

        currentLength = length; // current length used by other scripts

        int randomSeed = Mathf.Clamp(levelNr * size * length, 0, Int32.MaxValue);   // setting seed as multiply of level number, size of map and length of path to minimize chance of path starting the same way
        SetRandom(randomSeed);

        int len = 1;    // keeps track of current length
        while (len < length)    // loops until path of desired length is reached
        {
            len = 1;
            InitializeMap(length);
            pos pos = RandomPosOnEdge();
            SetPath(pos, len - 1);

            while (len < length && PossibleToExtend(pos) != directions.none)
            {
                directions dir = PossibleToExtend(pos);
                pos = Move(pos, dir);
                len++;
                SetPath(pos, len - 1);
            }
        }
    }

    private directions PossibleToExtend(pos pos) {
        if ((pos.x - 1 < 0) && (pos.y - 1 < 0) && (pos.x + 1 >= GetSize()) && (pos.y + 1 >= GetSize()))
            return directions.none;


        List<directions> possibilities = new List<directions>();

        if (pos.x + 1 < GetSize() && GetMapAt(pos.x + 1, pos.y) == empty)
            possibilities.Add(directions.down);
        if (pos.x - 1 >= 0 && GetMapAt(pos.x - 1, pos.y) == empty)
            possibilities.Add(directions.up);
        if (pos.y + 1 < GetSize() && GetMapAt(pos.x, pos.y + 1) == empty)
            possibilities.Add(directions.right);
        if (pos.y - 1 >= 0 && GetMapAt(pos.x, pos.y - 1) == empty)
            possibilities.Add(directions.left);

        if (possibilities.Count == 0)
            return directions.none;
        else
            return possibilities[random.Next(possibilities.Count)];
    }

    private pos RandomPosOnEdge() {
        int side = random.Next(4);
        pos pos;
        switch (side)
        {
            case 0: // left
                pos.x = random.Next(GetSize());
                pos.y = 0;
                break;
            case 1: // right
                pos.x = random.Next(GetSize());
                pos.y = GetSize() - 1;
                break;
            case 2: // up
                pos.x = 0;
                pos.y = random.Next(GetSize());
                break;
            case 3: // down
                pos.x = GetSize() - 1;
                pos.y = random.Next(GetSize());
                break;
            default:// just in case
                pos.x = 0;
                pos.y = 0;
                break;
        }
        return pos;
    }

    public void SetSize(int s) { size = s; }
    private int GetSize() { return size; }

    private char GetMapAt(int i, int j) { return map[i][j]; }

    private void SetPath(pos pos) { map[pos.x][pos.y] = (char)(pathSign++ % 9 + 49); }
    private void SetPath(pos pos, int ind) {
        map[pos.x][pos.y] = (char)(pathSign++ % 9 + 49);
        cordi[ind] = pos.x;
        cordj[ind] = pos.y;
    }

    public void SetMinLength(int i) { lengthMin = i; }
    public void SetMaxLength(int i) { lengthMax = i; }

    private pos Move(pos pos, directions dir) {
        switch (dir)
        {
            case directions.left:
                if (pos.y - 1 >= 0)
                    pos.y--;
                break;
            case directions.right:
                if (pos.y + 1 < GetSize())
                    pos.y++;
                break;
            case directions.up:
                if (pos.x - 1 >= 0)
                    pos.x--;
                break;
            case directions.down:
                if (pos.x + 1 < GetSize())
                    pos.x++;
                break;
            default:
                break;
        }
        return pos;
    }

    public int GetFirstCord(int ind) { return cordi[ind]; }
    public int GetSecondCord(int ind) { return cordj[ind]; }

    public int GetCurrentLength() { return currentLength; }
}