using ParserLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PrimaPower.Resource
{
    public class Tracer
    {
        UIElementCollection PathsCollection { get; set; }
        List<Path> TracedPathList { get; set; } = new List<Path>();
        IProgramContext ProgramContext { get; set; }
        int FirstSourceLine { get; set; }
        int LastSourceLine { get; set; }

        int LastTracedLine { get; set; }
        bool IsTraceForward { get; set; }

        Path TracingPath { get; set; } = new Path();
        Path LastTracedPath { get; set; } = new Path();

        SolidColorBrush TracingColor = Brushes.Orange;

        public Action TraceStep;
        public Action TraceRestarted;
        public Action TraceEnded;




        public void SetPathsCollection(UIElementCollection coll)
        {
            PathsCollection = coll;
        }
        public void SetProgramContext(IProgramContext pC)
        {
            ProgramContext = pC;
            FirstSourceLine = pC.FirstSourceLine;
            LastSourceLine = pC.LastSourceLine;

        }

        public void RestartTrace(int fromsourceLine)
        {
            int pathID = GetPathIDFromSourceLine(fromsourceLine);
            for (int i =0; i< fromsourceLine; i++)
            {
                TraceLine(i);
            }
            TraceRestarted?.Invoke();
        }

        public void TraceLine(int sourceLine)
        {
            GetTraceDirection(sourceLine);
            int pathID = GetPathIDFromSourceLine(sourceLine);

            if (pathID == -1) return;

            Path child = PathsCollection[pathID] as Path;
            IBaseEntity move = child.Tag as IBaseEntity;

            if (move.IsBeamOn == true)
            {
                TracingPath = child as Path;
                TracingPath.Stroke = TracingColor;

                if (LastTracedPath.Tag != null)
                {
                    IBaseEntity oldMove = LastTracedPath.Tag as IBaseEntity;
                    if (IsTraceForward)
                        LastTracedPath.Stroke = ColorsHelper.GetLineColorTrace(oldMove.LineColor);
                    else
                        LastTracedPath.Stroke = ColorsHelper.GetLineColor(oldMove.LineColor);
                }
                TracedPathList.Add(TracingPath);
                LastTracedPath = TracingPath;
                //TraceStep.Invoke();
            }
        }

        public void EndTrace()
        {
            Path lastPath = TracedPathList.Last();
            IBaseEntity move = lastPath.Tag as IBaseEntity;
            LastTracedPath.Stroke = ColorsHelper.GetLineColorTrace(move.LineColor);
            TraceEnded?.Invoke();
        }

        private int GetPathIDFromSourceLine(int sourceLine)
        {
            for (int i = 0; i < PathsCollection.Count; i++)
            {
                Path child = PathsCollection[i] as Path;
                if ((child.Tag as IBaseEntity).SourceLine == sourceLine)
                {
                    return i;
                }
            }
            return -1;
        }
        private void GetTraceDirection(int sourceLine)
        {
            if (sourceLine > LastTracedLine)
                IsTraceForward = true;
            else
                IsTraceForward = false;

            LastTracedLine = sourceLine;
        }


    }
}
