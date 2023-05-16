using UnityEngine;

namespace CagrsLib.Archive.Scripts
{
    public class TransformData : MonoBehaviour
    {
        public ArchiveSetting position;
        public ArchiveSetting rotation;
        public ArchiveSetting scale;

        public bool clearVelocityOnLoad;

        [ArchiveData.Write]
        public ArchivePack WriteTransform()
        {
            ArchivePack pack = ArchivePack.CreatePack("transform");
            
            if (position.write) pack.Write("position", Converter.Convert(transform.position));
            
            if (rotation.write) pack.Write("rotation", Converter.Convert(transform.rotation));
            
            if (scale.write) pack.Write("scale", Converter.Convert(transform.localScale));

            return pack;
        }

        [ArchiveData.Load("transform")]
        public void LoadTransform(ArchivePack pack)
        {
            if (position.load) transform.position = Converter.Convert((Vector3Data) pack.Load("position"));
            
            if (rotation.load) transform.rotation = Converter.Convert((QuaternionData) pack.Load("rotation"));
            
            if (scale.load) transform.localScale = Converter.Convert((Vector3Data) pack.Load("scale"));

            if (clearVelocityOnLoad)
            {
                Rigidbody component = GetComponent<Rigidbody>();
                if (component != null)
                {
                    component.velocity = Vector3.zero;
                }
            }
        }
    }
}
