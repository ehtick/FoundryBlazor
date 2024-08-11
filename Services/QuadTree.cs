using Blazor.Extensions.Canvas.Canvas2D;
using FoundryRulesAndUnits.Extensions;
using System.Drawing;

// https://happycoding.io/tutorials/processing/collision-detection
// https://bytes.com/topic/c-sharp/insights/880968-generic-quadtree-implementation

namespace FoundryBlazor.Shape;


public class QuadTree<T> where T : QuadHitTarget
{
    private static readonly Queue<QuadTree<T>> cashe = new();
    public static QuadTree<T> NewQuadTree(int x, int y, int width, int height)
    {
        if (cashe.Count == 0)
        {
            //$"New QuadTree {cashe.Count}".WriteInfo();
            return new QuadTree<T>(x, y, width, height);
        }

        //$"Recycle QuadTree {cashe.Count}".WriteInfo();
        var result = cashe.Dequeue();
        result.Reset(x, y, width, height);
        return result;
    }

    public static QuadTree<T>? SmashQuadTree(QuadTree<T>? source)
    {
        if (source == null) return null;

        source.Clear(true);
        cashe.Enqueue(source);
       // $"Smash QuadTree {cashe.Count}".WriteNote();
        return null;
    }

    #region Constants
    // How many objects can exist in a QuadTree before it sub divides itself
    private const int MAX_OBJECTS_PER_NODE = 2;
    #endregion

    #region Private Members
    private List<QuadHitTarget>? m_objects = null;       // The objects in this QuadTree
    private Rectangle m_rect;               // The area this QuadTree represents

    private QuadTree<T>? m_childTL = null;   // Top Left Child
    private QuadTree<T>? m_childTR = null;   // Top Right Child
    private QuadTree<T>? m_childBL = null;   // Bottom Left Child
    private QuadTree<T>? m_childBR = null;   // Bottom Right Child
    #endregion

    #region Public Properties

    public Rectangle QuadRect { get { return m_rect; } }


    public QuadTree<T>? TopLeftChild { get { return m_childTL; } }


    public QuadTree<T>? TopRightChild { get { return m_childTR; } }


    public QuadTree<T>? BottomLeftChild { get { return m_childBL; } }


    public QuadTree<T>? BottomRightChild { get { return m_childBR; } }


    public List<QuadHitTarget>? Members() {return m_objects; }

    public List<Rectangle> HitRectangles() 
    { 
        return m_objects?.Where(obj => obj.IsRectangle()).Select(obj => obj.rect).ToList() ?? new List<Rectangle>(); 
    }


    public int Count { get { return this.ObjectCount(); } }
    #endregion

    #region Constructor

    public QuadTree(Rectangle rect)
    {
        m_rect = rect;
    }
    public QuadTree(int x, int y, int width, int height)
    {
        m_rect = new Rectangle(x, y, width, height);
    }

    public void PrintTree(int level = 0)
    {
        $"PrintTree {m_rect} {Count}".WriteSuccess(level);
        if (m_childTL != null) m_childTL.PrintTree(level + 1);
        if (m_childTR != null) m_childTR.PrintTree(level + 1);
        if (m_childBL != null) m_childBL.PrintTree(level + 1);
        if (m_childBR != null) m_childBR.PrintTree(level + 1);
    }


    public QuadTree<T> Reset(int x, int y, int width, int height)
    {
        m_rect.X = x;   
        m_rect.Y = y;
        m_rect.Width = width;
        m_rect.Height = height;
        return this;
    }

    public bool IsSmashed()
    {
        var count = m_objects?.Count(obj => obj.target.IsSmashed());
        var smashed = count > 0;
        if ( !HasSubTrees() ) 
            return smashed;

        if ( m_childTL!.IsSmashed() || m_childTR!.IsSmashed() || m_childBL!.IsSmashed() || m_childBR!.IsSmashed() )
            smashed = true;

        return smashed;
    }

    #endregion

    public async Task DrawQuadTree(Canvas2DContext ctx, bool members = false)
    {

        await ctx.SetStrokeStyleAsync("Green");
        await ctx.StrokeRectAsync(m_rect.X+1, m_rect.Y+1, m_rect.Width-2, m_rect.Height-2);

        if ( members )
        {
            await ctx.SaveAsync();
            await ctx.SetFillStyleAsync("Yellow");
            await ctx.SetGlobalAlphaAsync(0.75f);
            m_objects?.ForEach(async item =>
            {
                if ( item.IsRectangle() )
                {
                    var rect = item.rect;
                    await ctx.FillRectAsync(rect.X+1, rect.Y+1, rect.Width-2, rect.Height-2);
                }
                else if ( item.IsLineSegment() )
                {
                    var rect = item.rect;
                    var point = item.point;
                    await ctx.BeginPathAsync();
                    await ctx.MoveToAsync(rect.X, rect.Y);
                    await ctx.LineToAsync(point.X, point.Y);
                    await ctx.StrokeAsync();
                }

            });
            await ctx.RestoreAsync();
        }

        if (TopLeftChild != null) await TopLeftChild.DrawQuadTree(ctx,members);
        if (TopRightChild != null) await TopRightChild.DrawQuadTree(ctx,members);
        if (BottomLeftChild != null) await BottomLeftChild.DrawQuadTree(ctx,members);
        if (BottomRightChild != null) await BottomRightChild.DrawQuadTree(ctx,members);
    }

    public QuadHitTarget FillTarget(Rectangle rect, ICanHitTarget target)
    {
        //in the future, reuse one that was purged
        return new QuadHitTarget(m_rect, target);
    }


    #region Private Members

    private void Add(ICanHitTarget item, Rectangle rect)
    {
        m_objects ??= new List<QuadHitTarget>();
        m_objects.Add(FillTarget(rect, item));
        //$"Tree Add {m_rect} {m_objects.Count} {item}".WriteInfo(2);
    }


    private void Remove(ICanHitTarget item)
    {
        if (m_objects == null) return;
        
        m_objects.RemoveAll(obj => obj.target.Equals(item));
    }


    private int ObjectCount()
    {
        int count = 0;

        // Add the objects at this level
        if (m_objects != null) count += m_objects.Count;

        // Add the objects that are contained in the children

        count += m_childTL == null ? 0 : m_childTL.ObjectCount();
        count += m_childTR == null ? 0 : m_childTR.ObjectCount();
        count += m_childBL == null ? 0 : m_childBL.ObjectCount();
        count += m_childBR == null ? 0 : m_childBR.ObjectCount();


        return count;
    }



    private void Subdivide()
    {
          //$"Tree Subdivide items".WriteInfo(2);

        // We've reached capacity, subdivide...
        Point size = new(m_rect.Width / 2, m_rect.Height / 2);
        Point mid = new(m_rect.X + size.X, m_rect.Y + size.Y);

        m_childTL = NewQuadTree(m_rect.Left, m_rect.Top, size.X, size.Y);
        m_childTR = NewQuadTree(mid.X, m_rect.Top, size.X, size.Y);
        m_childBL = NewQuadTree(m_rect.Left, mid.Y, size.X, size.Y);
        m_childBR = NewQuadTree(mid.X, mid.Y, size.X, size.Y);

        // If they're completely contained by the quad, bump objects down
        for (int i = 0; i < m_objects?.Count; i++)
        {
            var item = m_objects[i].target;
            var itemRect = m_objects[i].rect;
            QuadTree<T> destTree = GetDestinationTree(item,itemRect);

            if (destTree != this)
            {
                // Insert to the appropriate tree, remove the object, and back up one in the loop
                destTree.Insert(item, itemRect);
                Remove(item);
                i--;
            }
        }
    }


    private QuadTree<T> GetDestinationTree(ICanHitTarget item, Rectangle rect)
    {
        // If a child can't contain an object, it will live in this Quad
        QuadTree<T> destTree = this;

        if (m_childTL!.QuadRect.Contains(rect))
        {
            destTree = m_childTL;
        }
        else if (m_childTR!.QuadRect.Contains(rect))
        {
            destTree = m_childTR;
        }
        else if (m_childBL!.QuadRect.Contains(rect))
        {
            destTree = m_childBL;
        }
        else if (m_childBR!.QuadRect.Contains(rect))
        {
            destTree = m_childBR;
        }

        return destTree;
    }
    #endregion

    #region Public Methods

    public QuadTree<T> Clear(bool force)
    {
        // the question is:  do we want to clear the tree if it has no smashed objects?

        //$"Tree Clear items {m_rect} {Count} {force}".WriteInfo(2);
        var smashed = IsSmashed();
        //no smashed objects, no need to clear
        if ( smashed && !force && !HasSubTrees()) 
            return this;

        // Clear any objects at this level
        if ( smashed || force)
            m_objects?.Clear();

        m_childTL?.Clear(force);
        m_childTR?.Clear(force);
        m_childBL?.Clear(force);
        m_childBR?.Clear(force);

        // Set the children to null
        if ( force )
        {
            m_childTL = SmashQuadTree(m_childTL);
            m_childTR = SmashQuadTree(m_childTR);
            m_childBL = SmashQuadTree(m_childBL);
            m_childBR = SmashQuadTree(m_childBR);
        }

        return this;
    }


    public void Delete(ICanHitTarget item)
    {
        // If this level contains the object, remove it
        bool objectRemoved = false;
        if (m_objects != null && m_objects.Count(obj => obj.target.Equals(item)) > 0)
        {
            Remove(item);
            objectRemoved = true;
        }

        // If we didn't find the object in this tree, try to delete from its children
        if (!objectRemoved)
        {
            m_childTL?.Delete(item);
            m_childTR?.Delete(item);
            m_childBL?.Delete(item);
            m_childBR?.Delete(item);
        }

        if (HasSubTrees())
        {
            // If all the children are empty, delete all the children
            if (m_childTL!.Count == 0 &&
                m_childTR!.Count == 0 &&
                m_childBL!.Count == 0 &&
                m_childBR!.Count == 0)
            {
                m_childTL = SmashQuadTree(m_childTL);
                m_childTR = SmashQuadTree(m_childTR);
                m_childBL = SmashQuadTree(m_childBL);
                m_childBR = SmashQuadTree(m_childBR);
            }
        }
    }


    public bool HasSubTrees()
    {
        return m_childTL != null;
    }
    /// Insert an item into this QuadTree object.

    public void Insert(ICanHitTarget item, Rectangle itemRect)
    {
        //$"Tree Inserting {item} items".WriteInfo(2);

        // If this quad doesn't intersect the items rectangle, do nothing
        if (!m_rect.IntersectsWith(itemRect))
            return;

        if (m_objects == null )
            Add(item,itemRect); // If there's room to add the object, just add it       
        else if ( m_objects.Count + 1 <= MAX_OBJECTS_PER_NODE && HasSubTrees() == false)
            Add(item,itemRect); // If there's room to add the object, just add it
        else
        {
            // No quads, create them and bump objects down where appropriate
            if ( !HasSubTrees() )
                Subdivide();
            

            // Find out which tree this object should go in and add it there
            var destTree = GetDestinationTree(item, itemRect);
            if (destTree == this)
                Add(item,itemRect);
            else
                destTree.Insert(item,itemRect);
        }
    }


    public void GetObjects(Rectangle rect, ref List<QuadHitTarget> results)
    {
        // We can't do anything if the results list doesn't exist
        if (results == null) return;
        
        if (rect.Contains(m_rect))
        {
            // If the search area completely contains this quad, just get every object this quad and all it's children have
            GetAllObjects(ref results);
        }
        else if (rect.IntersectsWith(m_rect))
        {
            // Otherwise, if the quad isn't fully contained, only add objects that intersect with the search rectangle
            if (m_objects != null)
            {
                //$"Tree Search Objects {m_rect} {m_objects.Count} {rect}".WriteInfo(2);
                for (int i = 0; i < m_objects.Count; i++)
                {
                    var obj = m_objects[i];
                    var hit = obj.rect;
                    if (rect.IntersectsWith(hit))
                        results.Add(obj);
                }
                    
            }

            // Get the objects for the search rectangle from the children

            m_childTL?.GetObjects(rect, ref results);
            m_childTR?.GetObjects(rect, ref results);
            m_childBL?.GetObjects(rect, ref results);
            m_childBR?.GetObjects(rect, ref results);
        }
        
    }


    public void GetAllObjects(ref List<QuadHitTarget> results)
    {
        // If this Quad has objects, add them
        if (m_objects != null)
            results.AddRange(m_objects);

        // If we have children, get their objects too

        m_childTL?.GetAllObjects(ref results);
        m_childTR?.GetAllObjects(ref results);
        m_childBL?.GetAllObjects(ref results);
        m_childBR?.GetAllObjects(ref results);

    }
    #endregion
}

