using System;
using System.Collections.Generic;
using System.Web.Http;

namespace VisPrWindowsDesktopRecorder.Algorithms
{
    public class RecordingController : ApiController
    {
        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/values 
        public void Post()
        {
        }

        // PUT api/values/5 
        public void Put()
        {
            MainWindow.Instance.OnClickStart(this, EventArgs.Empty);
        }

        // DELETE api/values/5 
        public void Delete()
        {
            MainWindow.Instance.OnClickStop(this, EventArgs.Empty);
        }
    }
}
