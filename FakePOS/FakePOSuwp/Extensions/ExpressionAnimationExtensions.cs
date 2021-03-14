using System;

#if WINDOWS_UWP
using Windows.UI.Xaml;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
#else
using Microsoft.UI.Xaml;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;
#endif

namespace FakePOS
{
    static public class ExpressionAnimationExtensions
    {
        static public ExpressionAnimationWrapper CreateExpressionWrapper(this Compositor compositor, string expression)
        {
            return new ExpressionAnimationWrapper(compositor, expression);
        }
    }

    public class ExpressionAnimationWrapper
    {
        public ExpressionAnimationWrapper(Compositor compositor, string expression)
        {
            Compositor = compositor;
            Expression = Compositor.CreateExpressionAnimation(expression);
        }

        public Compositor Compositor { get; private set; }

        public ExpressionAnimation Expression { get; private set; }

        public ExpressionAnimationWrapper Parameter(string key, float value)
        {
            Expression.SetScalarParameter(key, value);
            return this;
        }

        public ExpressionAnimationWrapper Parameter(string key, CompositionObject compositionObject)
        {
            Expression.SetReferenceParameter(key, compositionObject);
            return this;
        }

        public ExpressionAnimationWrapper Start(UIElement element, string propertyName)
        {
            return Start(ElementCompositionPreview.GetElementVisual(element), propertyName);
        }

        public ExpressionAnimationWrapper Start(Visual visual, string propertyName)
        {
            visual.StartAnimation(propertyName, Expression);
            return this;
        }
    }
}
