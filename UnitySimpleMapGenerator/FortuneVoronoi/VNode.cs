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

    using NGenerics.DataStructures.Mathematical;

    internal abstract class VNode
    {
        private VNode _Parent = null;

        private VNode _Left = null;

        private VNode _Right = null;

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>
        /// The left.
        /// </value>
        public VNode Left {
            get {
                return _Left;
            }

            set {
                _Left = value;
                value.Parent = this;
            }
        }

        /// <summary>
        /// Gets or sets the right.
        /// </summary>
        /// <value>
        /// The right.
        /// </value>
        public VNode Right {
            get {
                return _Right;
            }

            set {
                _Right = value;
                value.Parent = this;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public VNode Parent {
            get {
                return _Parent;
            }

            set {
                _Parent = value;
            }
        }

        /// <summary>
        /// Replace the specified ChildOld and ChildNew.
        /// </summary>
        /// <param name='ChildOld'>
        /// Child old.
        /// </param>
        /// <param name='ChildNew'>
        /// Child new.
        /// </param>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public void Replace( VNode ChildOld, VNode ChildNew )
        {
            if( Left == ChildOld ) {
                Left = ChildNew;
            }
            else if( Right == ChildOld ) {
                Right = ChildNew;
            }
            else {
                throw new Exception( "Child not found!" );
            }

            ChildOld.Parent = null;
        }

        /// <summary>
        /// Firsts the data node.
        /// </summary>
        /// <returns>
        /// The data node.
        /// </returns>
        /// <param name='Root'>
        /// Root.
        /// </param>
        public static VDataNode FirstDataNode( VNode Root )
        {
            VNode C = Root;
            while( C.Left!=null ) {
                C = C.Left;
            }

            return C as VDataNode;
        }

        /// <summary>
        /// Lefts the data node.
        /// </summary>
        /// <returns>
        /// The data node.
        /// </returns>
        /// <param name='Current'>
        /// Current.
        /// </param>
        public static VDataNode LeftDataNode( VDataNode Current )
        {
            VNode C = Current;

            //1. Up
            do {
                if( C.Parent == null ) {
                    return null;
                }
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
            while( C.Right != null ) {
                C = C.Right;
            }

            return C as VDataNode; // Cast statt 'as' damit eine Exception kommt
        }

        /// <summary>
        /// Rights the data node.
        /// </summary>
        /// <returns>
        /// The data node.
        /// </returns>
        /// <param name='Current'>
        /// Current.
        /// </param>
        public static VDataNode RightDataNode( VDataNode Current )
        {
            VNode C = Current;

            //1. Up
            do {
                if( C.Parent == null ) {
                    return null;
                }
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
            while( C.Left != null ) {
                C = C.Left;
            }

            return C as VDataNode; // Cast statt 'as' damit eine Exception kommt
        }

        /// <summary>
        /// Edges to right data node.
        /// </summary>
        /// <returns>
        /// The to right data node.
        /// </returns>
        /// <param name='Current'>
        /// Current.
        /// </param>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public static VEdgeNode EdgeToRightDataNode( VDataNode Current )
        {
            VNode C = Current;

            //1. Up
            do {
                if( C.Parent == null ) {
                    throw new Exception( "No Left Leaf found!" );
                }
                if( C.Parent.Right == C ) {
                    C = C.Parent;
                    continue;
                } else {
                    C = C.Parent;
                    break;
                }
            } while(true);

            return C as VEdgeNode;
        }

        /// <summary>
        /// Finds the data node.
        /// </summary>
        /// <returns>
        /// The data node.
        /// </returns>
        /// <param name='Root'>
        /// Root.
        /// </param>
        /// <param name='ys'>
        /// Ys.
        /// </param>
        /// <param name='x'>
        /// X.
        /// </param>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public static VDataNode FindDataNode( VNode Root, double ys, double x )
        {
            VNode C = Root;

            do {
                if( C is VDataNode ) {
                    return C as VDataNode;
                }

                VEdgeNode cEdge = C as VEdgeNode;
                if( cEdge == null ) {
                    throw new Exception( "Can't cast to edge node" );
                }
                if( cEdge.Cut( ys, x ) < 0 ) {
                    C = C.Left;
                }
                else {
                    C = C.Right;
                }
            } while(true);
        }

        /// <summary>
        /// Will return the new root (unchanged except in start-up)
        /// </summary>
        /// <summary>
        /// Processes the data event.
        /// </summary>
        /// <returns>
        /// The data event.
        /// </returns>
        /// <param name='e'>
        /// E.
        /// </param>
        /// <param name='Root'>
        /// Root.
        /// </param>
        /// <param name='VG'>
        /// V.
        /// </param>
        /// <param name='ys'>
        /// Ys.
        /// </param>
        /// <param name='CircleCheckList'>
        /// Circle check list.
        /// </param>
        /// <exception cref='Exception'>
        /// Represents errors that occur during application execution.
        /// </exception>
        public static VNode ProcessDataEvent( VDataEvent e, VNode Root, VoronoiGraph VG, double ys, out VDataNode[] CircleCheckList )
        {
            if( Root == null ) {
                Root = new VDataNode( e.DataPoint );
                CircleCheckList = new VDataNode[] {Root as VDataNode};
                return Root;
            }

            //1. Find the node to be replaced
            VNode C = VNode.FindDataNode( Root, ys, e.DataPoint.X );
            VDataNode cData = C as VDataNode;

            if( cData == null ) {
                throw new Exception( "Can't cast to data node" );
            }

            //2. Create the subtree (ONE Edge, but two VEdgeNodes)
            VoronoiEdge VE = new VoronoiEdge();
            VE.LeftData = cData.DataPoint;
            VE.RightData = e.DataPoint;
            VE.VVertexA = Fortune.VVUnkown;
            VE.VVertexB = Fortune.VVUnkown;
            VG.Edges.Add( VE );

            VNode SubRoot;
            if( Math.Abs( VE.LeftData.Y - VE.RightData.Y ) < 1e-10 ) {
                if( VE.LeftData.X < VE.RightData.X ) {
                    SubRoot = new VEdgeNode( VE, false );
                    SubRoot.Left = new VDataNode( VE.LeftData );
                    SubRoot.Right = new VDataNode( VE.RightData );
                } else {
                    SubRoot = new VEdgeNode( VE, true );
                    SubRoot.Left = new VDataNode( VE.RightData );
                    SubRoot.Right = new VDataNode( VE.LeftData );
                }

                CircleCheckList = new VDataNode[] {SubRoot.Left as VDataNode, SubRoot.Right as VDataNode};
            } else {
                SubRoot = new VEdgeNode( VE, false );
                SubRoot.Left = new VDataNode( VE.LeftData );
                SubRoot.Right = new VEdgeNode( VE, true );
                SubRoot.Right.Left = new VDataNode( VE.RightData );
                SubRoot.Right.Right = new VDataNode( VE.LeftData );
                CircleCheckList = new VDataNode[] {SubRoot.Left as VDataNode, SubRoot.Right.Left as VDataNode, SubRoot.Right.Right as VDataNode};
            }

            //3. Apply subtree
            if( C.Parent == null ) {
                return SubRoot;
            }

            C.Parent.Replace( C, SubRoot );

            return Root;
        }

        /// <summary>
        /// Processes the circle event.
        /// </summary>
        /// <returns>
        /// The circle event.
        /// </returns>
        /// <param name='e'>
        /// E.
        /// </param>
        /// <param name='Root'>
        /// Root.
        /// </param>
        /// <param name='VG'>
        /// V.
        /// </param>
        /// <param name='ys'>
        /// Ys.
        /// </param>
        /// <param name='CircleCheckList'>
        /// Circle check list.
        /// </param>
        public static VNode ProcessCircleEvent( VCircleEvent e, VNode Root, VoronoiGraph VG, double ys, out VDataNode[] CircleCheckList )
        {
            VDataNode a;
            VDataNode b;
            VDataNode c;
            VEdgeNode eu;
            VEdgeNode eo;

            b = e.NodeN;
            a = VNode.LeftDataNode( b );
            c = VNode.RightDataNode( b );
            if( a == null || b.Parent == null || c == null || !a.DataPoint.Equals( e.NodeL.DataPoint ) || !c.DataPoint.Equals( e.NodeR.DataPoint ) ) {
                CircleCheckList = new VDataNode[]{};

                return Root; // Abbruch da sich der Graph ver 0/00ndert hat
            }

            eu = b.Parent as VEdgeNode;
            CircleCheckList = new VDataNode[] {a,c};

            //1. Create the new Vertex
            Vector2D VNew = new Vector2D( e.Center.X, e.Center.Y );

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
            if( eo.Parent == null ) {
                return VEN;
            }

            eo.Parent.Replace( eo, VEN );

            return Root;
        }

        /// <summary>
        /// Circles the check data node.
        /// </summary>
        /// <returns>
        /// The check data node.
        /// </returns>
        /// <param name='n'>
        /// N.
        /// </param>
        /// <param name='ys'>
        /// Ys.
        /// </param>
        public static VCircleEvent CircleCheckDataNode( VDataNode n, double ys )
        {
            VDataNode l = VNode.LeftDataNode( n );
            VDataNode r = VNode.RightDataNode( n );
            if( l == null || r == null || l.DataPoint == r.DataPoint || l.DataPoint == n.DataPoint || n.DataPoint == r.DataPoint ) {
                return null;
            }

            if( MathTools.ccw( l.DataPoint.X, l.DataPoint.Y, n.DataPoint.X, n.DataPoint.Y, r.DataPoint.X, r.DataPoint.Y, false ) <= 0 ) {
                return null;
            }

            Vector2D Center = Fortune.CircumCircleCenter( l.DataPoint, n.DataPoint, r.DataPoint );
            VCircleEvent VC = new VCircleEvent();
            VC.NodeN = n;
            VC.NodeL = l;
            VC.NodeR = r;
            VC.Center = Center;
            VC.Valid = true;
            if( VC.Y >= ys ) {
                return VC;
            }

            return null;
        }
    }
}

