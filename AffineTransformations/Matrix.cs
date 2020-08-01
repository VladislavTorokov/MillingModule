namespace MillingModule
{
    public class Matrix
    {
        public float[,] elements = new float[4, 4];

        public Matrix()
        {
            for (int j = 0; j < 4; j++)
                for (int i = 0; i < 4; i++)
                {
                    if (i == j)
                        elements[i, j] = 1;
                }
        }

        public float[] Multiplication(float[] matrix)
        {
            float[] resultMatrix = new float[4];

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < matrix.Length; j++)
                    resultMatrix[i] += matrix[j] * elements[j, i];
            }
            return resultMatrix;
        }
    }
}
