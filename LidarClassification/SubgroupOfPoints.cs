using OpenTK;

namespace LidarClassification
{
    public class SubgroupOfPoints
    {
        public int classIndex;
        public float avgDistance;
        public float avgIntensity;
        public Vector3 slopeVector;
        public float avgHeight;

        public SubgroupOfPoints()
        {
            classIndex = -1;
            avgDistance = -1;
            avgIntensity = -1;
            avgHeight = -1;
            slopeVector = new Vector3(-1, -1, -1);
        }

        public SubgroupOfPoints(int classIndex)
        {
            this.classIndex = classIndex;
            avgDistance = -2;
            avgIntensity = -2;
            avgHeight = -2;
            slopeVector = new Vector3(-2, -2, -2);
        }

        public SubgroupOfPoints(int classInd, float avgDist, float avgInt, Vector3 slopeVec, float avgHeig)
        {
            classIndex = classInd;
            avgDistance = avgDist;
            avgIntensity = avgInt;
            slopeVector = slopeVec;
            avgHeight = avgHeig;
        }
    }
}