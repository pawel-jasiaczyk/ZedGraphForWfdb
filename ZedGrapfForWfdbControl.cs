using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;
using WfdbCsharpWrapper;
using System.IO;

namespace ZedGraphForWfdb
{
    /// <summary>
    /// This class extends ZedGraphControl for methods enabled opening and saving
    /// waveforms in PhysioBank format.
    /// </summary>
    public class ZedGraphForWfdbiControl : ZedGraphControl
    {
        #region Variables

        private Record record;
        private List<PointPairList> points;
        private bool isWfdbPathSet = false;
        private string tempPath = "";

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets array of PointPairLists for ZedGraph made from WFDB record
        /// </summary>
        public PointPairList[] Points { get { return this.points.ToArray(); } }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of ZedGraphForWfdbControl
        /// </summary>
        public ZedGraphForWfdbiControl() : base()
        {
            this.points = new List<PointPairList>();
            this.isWfdbPathSet = IsWfdbPathSetInSystem();
        }

        #endregion

        #region Open record methods
        
        /// <summary>
        /// Opens WFDB record and returns PointPairList array
        /// with dataset for ZedGraph
        /// 
        /// Database location must be set first!!!
        /// </summary>
        /// <param name="path">Path to files you want to read</param>
        /// <returns>Array of PointPairList</returns>
        public PointPairList[] GetProbesFromRecord(string path)
        {
            try
            {
                OpenRecordFile(path);
                return this.Points;
            }
            catch 
            {
                throw;
            }
            finally
            {
                if (!Object.ReferenceEquals(this.record, null)) record.Dispose();
            }
        }
        
        /// <summary>
        /// Opens new record from selected path
        /// Database location must be set first!!!!
        /// </summary>
        /// <param name="path"></param>
        public void OpenRecord(string path)
        {
            int i = 0;
            try
            {
                OpenRecordFile(path);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!Object.ReferenceEquals(this.record, null)) record.Dispose();
            }
        }
        
        private void OpenRecordFile(string path)
        {
            if (!this.isWfdbPathSet)
                throw new WfdbPathException("Path to database is not set!");
            
            try
            {
                string name = Path.GetFileNameWithoutExtension(path);
                this.record = new Record(name);
                record.Open();

                foreach(Signal signal in record.Signals)
                {
                    PointPairList temp = new PointPairList();
                    List<WfdbCsharpWrapper.Sample> samples = signal.ReadAll().ToList();
                    for (int i = 0; i < signal.NumberOfSamples; i++ )
                    {
                        temp.Add(i, samples[i]);   
                    }

                    this.points.Add(temp);
                }
            }
            catch 
            {
                throw;
            }
        }

        #endregion

        #region Database location methods

        /// <summary>
        /// Set this path as database location
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SetDataBaseLocation(string path)
        {
            if (IsWfdbPathCorrect(path))
            {
                try
                {
                    Wfdb.WfdbPath = path;
                    this.isWfdbPathSet = true;
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else
                return false;
        }
        /// <summary>
        /// Adds ew path as database location
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool AddDataBaseLocation(string path)
        {
            if (IsWfdbPathCorrect(path))
            {
                try
                {
                    String lastPath = Wfdb.WfdbPath;
                    Wfdb.WfdbPath = Wfdb.WfdbPath + ";" + path;
                    this.isWfdbPathSet = true;
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            else 
                return false;
        }

        // TODO - implement this
        private bool IsWfdbPathCorrect(string path)
        {
            // According to WFDB documentation, environtment variables should have white 
            // spaces in names of catalogs
            //
            // Of cource we have to be shure, that his path exist in system

            return true;
        }

        private bool IsWfdbPathSetInSystem()
        {
            string paths =
                Environment.GetEnvironmentVariable("WFDB", EnvironmentVariableTarget.Process);
            if (!String.IsNullOrEmpty(paths))
            {
                if(IsWfdbPathCorrect(paths)) return true;
            }
            return false;
        }
        

        /// <summary>
        /// Do not implemented
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SetTempLocation(string path)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
