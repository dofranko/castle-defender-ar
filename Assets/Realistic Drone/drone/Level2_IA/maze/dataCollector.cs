using UnityEngine;
using System.Collections.Generic;

public class dataCollector : MonoBehaviour {
    /// <summary>
    /// Structure that represents real coordinates in the data matrix
    /// </summary>
    public struct indx {
        public int x;
        public int z;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x">X position in the data matrix</param>
        /// <param name="z">Z position in the data matrix</param>
        public indx(int x, int z) { this.x = x; this.z = z; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="v">Vector3 from which we'll extract the X and Z to create an Indx</param>
        public indx(Vector3 v) { this.x = (int) v.x; this.z = (int)v.z; }

        /// <summary>
        /// Sum of Indxs
        /// </summary>
        /// <param name="i">Other Indx we'll sum to this</param>
        /// <returns>Returns the indx resulting from the sum of the current Indx and the one passed as argument</returns>
        public indx add(indx i) { return new indx(x + i.x, z + i.z); }

        /// <summary>
        /// Subtraction of Indxs
        /// </summary>
        /// <param name="i">Other Indx we'll subtract to this</param>
        /// <returns>Returns the indx resulting from the subtraction of the Indx passed as argument to the this Indx</returns>
        public indx subtract(indx i) { return new indx(x - i.x, z - i.z); }

        /// <summary>
        /// Multiplication of Indxs
        /// </summary>
        /// <param name="i">Other Indx we'll multiply to this</param>
        /// <returns>Returns the indx resulting from the multiplication of the current Indx and the one passed as argument</returns>
        public indx multiply(int i) { return new indx(x * i, z * i); }

        /// <summary>
        /// Scalar division
        /// </summary>
        /// <param name="i">Scalar value we will divide the Indx to</param>
        /// <returns>Returns a new Vector3, resulting from the division of this and the scalar passed as argument</returns>
        public Vector3 divideFloat(float i) { return multiplyFloat(1 / i); }

        /// <summary>
        /// Scalar multiplication
        /// </summary>
        /// <param name="i">Scalar value we will multiply to Indx</param>
        /// <returns>Returns a new Vector3, resulting from the multiplication of this and the scalar passed as argument</returns>
        public Vector3 multiplyFloat(float f) { return new Vector3(x * f, 0, z * f); }

        /// <summary>
        /// Converts the indx to a Vector3
        /// </summary>
        /// <returns>Returns a Vector3 obtained from the Indx</returns>
        public Vector3 ToVector3() { return multiplyFloat(1f); }

        /// <summary>
        /// Returns a nicely formatted string for this object
        /// </summary>
        public override string ToString() { return "[x:" + x + ",z:" + z +"]"; }

        /// <summary>
        /// Returns true if the two objects are equals
        /// </summary>
        public static bool Equals(indx i1, indx i2) { return (i1.x == i2.x && i1.z == i2.z); }
    }
    public class Data
    {                      
        Vector3 zeroPoint;
        indx zeroPointInData;
        /// <summary>
        /// Gets the zero-point in the matrix. This is used to calculate all other points
        /// </summary>
        /// <returns></returns>
        public indx getZeroPointInData() { return zeroPointInData; }
        float maxExplorableDistance;
        int arraySize;
        /// <summary>
        /// Gets the size of the matrix
        /// </summary>
        public int getArraySize() { return arraySize; }

        public byte[,] dataArray;

        /// <summary>
        /// Constructor
        /// <para>Associates 'Vector3 zeroPoint' to 'Indx zeroPointInData' </para>
        /// </summary>
        /// <param name="dataArraySize">Size of the matrix</param>
        /// <param name="maxExplorableDistance">Size of the area we can explore</param>
        /// <param name="zeroPoint">Initial point represented by world-coordinates </param>
        /// <param name="zeroPointInData">Initial point represented by an indx</param>
        public Data(int dataArraySize, float maxExplorableDistance, Vector3 zeroPoint, indx zeroPointInData) {
            dirtyValues = new LinkedList<indx>();

            arraySize = dataArraySize;
            dataArray = new byte[dataArraySize, dataArraySize];

            for (int i = 0; i < dataArraySize; i++)
                for (int j = 0; j < dataArraySize; j++)
                    dataArray[i, j] = 2;

            this.zeroPoint = zeroPoint;
            this.zeroPointInData = zeroPointInData;
            this.maxExplorableDistance = maxExplorableDistance;
        }

        /// <summary>
        /// Converts the real-world coordinates specified by the Vector3 to an Indx in the matrix
        /// </summary>
        /// <param name="v">Vector3 we want to convert</param>
        public indx v3ToIndx(Vector3 v) { return new indx((v - zeroPoint) / maxExplorableDistance * arraySize).add(zeroPointInData); }
        
        /// <summary>
        /// Converts the Indx in the matrix to a real-world coordinate
        /// </summary>
        /// <param name="i">Indx we want to convert</param>
        public Vector3 indxToV3(indx i) { return i.subtract(zeroPointInData).multiplyFloat(maxExplorableDistance / arraySize) + zeroPoint; }

        // list where we save the indxs that have been modified recently
        LinkedList<indx> dirtyValues;
        /// <summary>
        /// Gets the list of values recently modified
        /// </summary>
        public LinkedList<indx> getDirtyValuesList() { return dirtyValues; }

        /// <summary>
        /// Sets the value of the byte in position <c>v3ToIndx(v)</c>
        /// </summary>
        /// <param name="v"></param>
        /// <param name="val"></param>
        /// <returns>Returns TRUE if the value has been changed, FALSE otherwise</returns>
        public bool setValue(Vector3 v, byte val) {
            indx i = v3ToIndx(v);
            if (dataArray[i.x, i.z] != val && dataArray[i.x, i.z] != 1)
            {
                if (val == 1) dirtyValues.AddLast(i); 
                dataArray[i.x, i.z] = val;
                return true;
            }
            return false;            
        }

        /// <summary>
        /// Gets the value in the matrix at position (x,z)
        /// </summary>
        /// <param name="x">First coordinate in the matrix</param>
        /// <param name="z">Second coordinate in the matrix</param>
        /// <returns>Returns the value in the matrix at position (x,z)</returns>
        private int getValue(int x, int z) { return dataArray[x, z]; }

        /// <summary>
        /// Gets the value in the matrix, corresponding to the real-world coordinates specified by the Vector3
        /// </summary>
        /// <param name="v">Vector3 specifying the real-world coordinates of the point we are interested</param>
        /// <returns>Returns the value in the matrix at position <c>v3ToIndx(v)</c></returns>
        public int getValue(Vector3 v) { indx i = v3ToIndx(v); return getValue(i.x, i.z); }

        /// <summary>
        /// Returns true if there are no walls or unknown cells in the square with center 'v' and specified radius
        /// </summary>
        /// <param name="v">Center taken in consideration to explore if the cells are all free</param>
        /// <param name="radius">Radius of the search</param>
        /// <returns>0 -> areAllFree; 1 -> there are walls; 2 -> no walls but unknown cells</returns>
        public int areAllFree(Vector3 v, int radius)
        {
            bool thereAreUnknown = false;
            
            indx center = v3ToIndx(v);
            for (int i = center.x - radius / 2; i <= center.x + radius / 2; i++)
                for (int j = center.z - radius / 2; j <= center.z + radius / 2; j++)
                    if (dataArray[i, j] == 1)
                        return 1;
                    else if (dataArray[i, j] == 2)
                        thereAreUnknown = true;
            
            return thereAreUnknown ? 2 : 0;
        }

    }

    const int dataMaxSize = 1000;
    const float maxExplorableDistance = 500;
    public Data data;
    public dataDrawer dW;

    public int ticket = 0;
    public indx[] dronesPositions;
    bool initialized = false;
    /// <summary>
    /// Each drone has to call this function to obtain a ticket used for updating their position in the drawer
    /// <para>The first drone that calls this function determine the zeroPointPosition</para>
    /// </summary>
    /// <param name="dronePosition">The actual position of the drone</param>
    /// <returns>Returns a ticket that will be used for updating the position</returns>
    public int initialize(Vector3 dronePosition)
    {
        if (!initialized)
        {
            // the first drone that register itself determine the zeroPointPosition, that correspond to the center of the map
            dronesPositions = new indx[10]; 
            indx actualPosition = new indx(dataMaxSize / 2, dataMaxSize / 2);
            data = new Data(dataMaxSize, maxExplorableDistance, dronePosition, actualPosition);
            dW.setData(data);
            initialized = true;
        }
        
        // other drones just takes a ticket to actualize their position 
        dronesPositions[ticket] = new indx(dronePosition);
        return ticket++;
    }

    /// <summary>
    /// Using the ticket obtained with the initialization, the drone can update its position
    /// </summary>
    /// <param name="ticket">Ticket obtained with the initialization</param>
    /// <param name="pos">The actual position of the drone</param>
    public void updatePosition (int ticket, Vector3 pos) { dronesPositions[ticket] = data.v3ToIndx(pos); }

    /// <summary>
    /// Adds data to the matrix
    /// </summary>
    /// <param name="v">Position of the data we want to add</param>
    /// <param name="b">Value registered from sensors that indicates if there is a wall or it is empty space</param>
    public void addDataToTheMatrix(Vector3 v, byte b) { data.setValue(v, b); }
    
    /// <summary>
    /// If the Indxs created with the Vectors passed are equals returns true, false otherwise.
    /// </summary>
    public bool sameIndx(Vector3 v1, Vector3 v2) { return indx.Equals(data.v3ToIndx(v1), data.v3ToIndx(v2)); }

}