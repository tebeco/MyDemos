using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.latencybusters.lbm;

namespace EventQsrc
{
    class EventQsrc
    {
        static void Main(string[] args)
        {
            LBMContext ctx = null; /* Context object: container for UM "instance". */
            LBMSource src = null; /* Source object: for sending messages. */
            LBMEventQueue evq = null; /* Event Queue object: process source events on app thread */
            SrcCB cb = null;


            /*** Create a source using an event queue ***/
            try
            {
                LBMTopic topic = null;
                LBMSourceAttributes srcAttr = null;

                ctx = new LBMContext();
                srcAttr = new LBMSourceAttributes();

                /* create callback to process events */
                cb = new SrcCB();

                /* define Event Queue. This allows us to process events on an  */
                /* application thread, as opposed to the default behavior,     */
                /* which is to process events on the (internal) context thread.*/
                evq = new LBMEventQueue();

                topic = ctx.allocTopic("test.topic", srcAttr);
                src = new LBMSource(ctx, topic, evq);
            }
            catch (LBMException ex)
            {
                System.Console.Error.WriteLine("Error initializing LBM objects: " + ex.Message);
                System.Environment.Exit(1);
            }


            /* run the event queue for 60 seconds       */
            /* all the source events will be processed  */
            /* in this thread
            evq.run(60000);

            try
            {
                src.close();
                ctx.close();
                evq.close();
            }
            catch (LBMException ex)
            {
                System.Console.Error.WriteLine("Error closing LBM objects: " + ex.Message);
                System.Environment.Exit(1);
            }

        } /* main */
        } /* class eventQ */
    }

    class SrcCB
    {
        public bool blocked = false;

        public int onSourceEvent(Object arg, LBMSourceEvent sourceEvent)
        {
            String clientname;

            switch (sourceEvent.type())
            {
                case LBM.SRC_EVENT_CONNECT:
                    clientname = sourceEvent.dataString();
                    System.Console.Out.WriteLine("Receiver connect " + clientname);
                    break;
                case LBM.SRC_EVENT_DISCONNECT:
                    clientname = sourceEvent.dataString();
                    System.Console.Out.WriteLine("Receiver disconnect " + clientname);
                    break;
                default:
                    break;
            }
            sourceEvent.dispose();
            System.Console.Out.Flush();
            return 0;
        }
    }

}
