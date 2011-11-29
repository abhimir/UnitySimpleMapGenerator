// Original code by BenDi on CodeProject:
//
// http://www.codeproject.com/KB/recipes/fortunevoronoi.aspx
//
// Cleaned up and adapted by Taffer for UnitySimpleMapGenerator on Github:
//
// https://github.com/Taffer/UnitySimpleMapGenerator

namespace BenDi.FortuneVoronoi
{
    using System;

    using UnityEngine;

    internal abstract class VNode
    {
        private VNode _Parent = null;
        private VNode _Left = null, _Right = null;

        public VNode Left {
            get{ return _Left;}
            set {
                _Left = value;
                value.Parent = this;
            }
        }

        public VNode Right {
            get{ return _Right;}
            set {
                _Right = value;
                value.Parent = this;
            }
        }

        public VNode Parent {
            get{ return _Parent;}
            set{ _Parent = value;}
        }

        public void Replace( VNode ChildOld, VNode ChildNew )
        {
            if( Left == ChildOld )
                Left = ChildNew;
            else if( Right == ChildOld )
                Right = ChildNew;
            else
                throw new Exception( "Child not found!" );
            ChildOld.Parent = null;
        }

        public static VDataNode FirstDataNode( VNode Root )
        {
            VNode C = Root;
            while( C.Left!=null )
                C = C.Left;
            return (VDataNode)C;
        }

        public static VDataNode LeftDataNode( VDataNode Current )
        {
            VNode C = Current;
            //1. Up
            do {
                if( C.Parent == null )
                    return null;
                if( C.Parent.Left == C ) {
                    C = C.Parent;
                    continue;
                } else {
                    C = C.Parent;
                    break;
                }
            } while(true);
            //2. One Left
            C = C.Left;
            //3. Down
            while( C.Right!=null )
                C = C.Right;
            return (VDataNode)C; // Cast statt 'as' damit eine Exception kommt
        }

        public static VDataNode RightDataNode( VDataNode Current )
        {
            VNode C = Current;
            //1. Up
            do {
                if( C.Parent == null )
                    return null;
                if( C.Parent.Right == C ) {
                    C = C.Parent;
                    continue;
                } else {
                    C = C.Parent;
                    break;
                }
            } while(true);
            //2. One Right
            C = C.Right;
            //3. Down
            while( C.Left!=null )
                C = C.Left;
            return (VDataNode)C; // Cast statt 'as' damit eine Exception kommt
        }

        public static VEdgeNode EdgeToRightDataNode( VDataNode Current )
        {
            VNode C = Current;
            //1. Up
            do {
                if( C.Parent == null )
                    throw new Exception( "No Left Leaf found!" );
                if( C.Parent.Right == C ) {
                    C = C.Parent;
                    continue;
                } else {
                    C = C.Parent;
                    break;
                }
            } while(true);
            return (VEdgeNode)C;
        }

        public static VDataNode FindDataNode( VNode Root, double ys, double x )
        {
            VNode C = Root;
            do {
                if( C is VDataNode )
                    return (VDataNode)C;
                if( ( (VEdgeNode)C ).Cut( ys, x ) < 0 )
                    C = C.Left;
                else
                    C = C.Right;
            } while(true);
        }

        /// <summary>
        /// Will return the new root (unchanged except in start-up)
        /// </summary>
        public static VNode ProcessDataEvent( VDataEvent e, VNode Root, VoronoiGraph VG, double ys, out VDataNode[] CircleCheckList )
        {
            if( Root == null ) {
                Root = new VDataNode( e.DataPoint );
                CircleCheckList = new VDataNode[] {(VDataNode)Root};
                return Root;
            }
            //1. Find the node to be replaced
            VNode C = VNode.FindDataNode( Root, ys, e.DataPoint[0] );
            //2. Create the subtree (ONE Edge, but two VEdgeNodes)
            VoronoiEdge VE = new VoronoiEdge();
            VE.LeftData = ( (VDataNode)C ).DataPoint;
            VE.RightData = e.DataPoint;
            VE.VVertexA = Fortune.VVUnkown;
            VE.VVertexB = Fortune.VVUnkown;
            VG.Edges.Add( VE );

            VNode SubRoot;
            if( Math.Abs( VE.LeftData[1] - VE.RightData[1] ) < 1e-10 ) {
                if( VE.LeftData[0] < VE.RightData[0] ) {
                    SubRoot = new VEdgeNode( VE, false );
                    SubRoot.Left = new VDataNode( VE.LeftData );
                    SubRoot.Right = new VDataNode( VE.RightData );
                } else {
                    SubRoot = new VEdgeNode( VE, true );
                    SubRoot.Left = new VDataNode( VE.RightData );
                    SubRoot.Right = new VDataNode( VE.LeftData );
                }
                CircleCheckList = new VDataNode[] {(VDataNode)SubRoot.Left,(VDataNode)SubRoot.Right};
            } else {
                SubRoot = new VEdgeNode( VE, false );
                SubRoot.Left = new VDataNode( VE.LeftData );
                SubRoot.Right = new VEdgeNode( VE, true );
                SubRoot.Right.Left = new VDataNode( VE.RightData );
                SubRoot.Right.Right = new VDataNode( VE.LeftData );
                CircleCheckList = new VDataNode[] {(VDataNode)SubRoot.Left,(VDataNode)SubRoot.Right.Left,(VDataNode)SubRoot.Right.Right};
            }

            //3. Apply subtree
            if( C.Parent == null )
                return SubRoot;
            C.Parent.Replace( C, SubRoot );
            return Root;
        }

        public static VNode ProcessCircleEvent( VCircleEvent e, VNode Root, VoronoiGraph VG, double ys, out VDataNode[] CircleCheckList )
        {
            VDataNode a, b, c;
            VEdgeNode eu, eo;
            b = e.NodeN;
            a = VNode.LeftDataNode( b );
            c = VNode.RightDataNode( b );
            if( a == null || b.Parent == null || c == null || !a.DataPoint.Equals( e.NodeL.DataPoint ) || !c.DataPoint.Equals( e.NodeR.DataPoint ) ) {
                CircleCheckList = new VDataNode[]{};
                return Root; // Abbruch da sich der Graph ver 0/00ndert hat
            }
            eu = (VEdgeNode)b.Parent;
            CircleCheckList = new VDataNode[] {a,c};
            //1. Create the new Vertex
            Vector2 VNew = new Vector2( e.Center.x, e.Center.y );
//           VNew[0] = Fortune.ParabolicCut(a.DataPoint[0],a.DataPoint[1],c.DataPoint[0],c.DataPoint[1],ys);
//           VNew[1] = (ys + a.DataPoint[1])/2 - 1/(2*(ys-a.DataPoint[1]))*(VNew[0]-a.DataPoint[0])*(VNew[0]-a.DataPoint[0]);
            VG.Vertizes.Add( VNew );
            //2. Find out if a or c are in a distand part of the tree (the other is then b's sibling) and assign the new vertex
            if( eu.Left == b ) { // c is sibling
                eo = VNode.EdgeToRightDataNode( a );

                // replace eu by eu's Right
                eu.Parent.Replace( eu, eu.Right );
            } else { // a is sibling
                eo = VNode.EdgeToRightDataNode( b );

                // replace eu by eu's Left
                eu.Parent.Replace( eu, eu.Left );
            }
            eu.Edge.AddVertex( VNew );
//           ///////////////////// uncertain
//           if(eo==eu)
//               return Root;
//           /////////////////////
            eo.Edge.AddVertex( VNew );
            //2. Replace eo by new Edge
            VoronoiEdge VE = new VoronoiEdge();
            VE.LeftData = a.DataPoint;
            VE.RightData = c.DataPoint;
            VE.AddVertex( VNew );
            VG.Edges.Add( VE );

            VEdgeNode VEN = new VEdgeNode( VE, false );
            VEN.Left = eo.Left;
            VEN.Right = eo.Right;
            if( eo.Parent == null )
                return VEN;
            eo.Parent.Replace( eo, VEN );
            return Root;
        }

        public static VCircleEvent CircleCheckDataNode( VDataNode n, double ys )
        {
            VDataNode l = VNode.LeftDataNode( n );
            VDataNode r = VNode.RightDataNode( n );
            if( l == null || r == null || l.DataPoint == r.DataPoint || l.DataPoint == n.DataPoint || n.DataPoint == r.DataPoint )
                return null;
            if( MathTools.ccw( l.DataPoint.x, l.DataPoint.y, n.DataPoint.x, n.DataPoint.y, r.DataPoint.x, r.DataPoint.y, false ) <= 0 )
                return null;
            Vector2 Center = Fortune.CircumCircleCenter( l.DataPoint, n.DataPoint, r.DataPoint );
            VCircleEvent VC = new VCircleEvent();
            VC.NodeN = n;
            VC.NodeL = l;
            VC.NodeR = r;
            VC.Center = Center;
            VC.Valid = true;
            if( VC.Y >= ys )
                return VC;
            return null;
        }
    }
}

