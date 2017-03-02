using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Rikka.Tsab2.Core.Services.Analytics
{
    public interface ICounter
    {
        string CounterType { get; set; }
    }

    public class HitCounter : ICounter
    {
        public HitCounter()
        {

        }
        public HitCounter(DateTime date)
        {
            HitDateTime = date;
        }
        public DateTime HitDateTime { get; set; }
        public string CounterType { get; set; } = "HitCounter";
    }
    public class EmotionalCounter: ICounter
    {
        public double Anger { get; set; }
        public double Contempt { get; set; }
        public double Disgust { get; set; }
        public double Fear { get; set; }
        public double Happiness { get; set; }
        public double Neutral { get; set; }
        public double Sadness { get; set; }
        public double Surprise { get; set; }

        public string CounterType { get; set; } = "EmotionalCounter";
    }

    public class ContextCounter: ICounter
    {
        public ContextCounter()
        {

        }


        public ContextCounter(string context, double count)
        {
            Context = context;
            Count = count;
        }

        public string Context { get; set; }
        public double Count { get; set; }
        public string CounterType { get; set; } = "ContextCounter";
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ContextCounter) obj);
        }

        protected bool Equals(ContextCounter other)
        {
            return string.Equals(Context, other.Context) && string.Equals(CounterType, other.CounterType);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Context?.GetHashCode() ?? 0) * 397) ^ (CounterType?.GetHashCode() ?? 0);
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Rikka.Tsab2.Core.Services.Analytics.ContextCounter" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(ContextCounter left, ContextCounter right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Rikka.Tsab2.Core.Services.Analytics.ContextCounter" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(ContextCounter left, ContextCounter right)
        {
            return !Equals(left, right);
        }
    }

    public class ThemeCounter: ICounter
    {
        public ThemeCounter()
        {
            
        }

        public ThemeCounter(string theme, double count)
        {
            Theme = theme;
            Count = count;
        }

        public string Theme { get; set; }
        public double Count { get; set; }
        public string CounterType { get; set; } = "ThemeCounter";

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ThemeCounter) obj);
        }

        protected bool Equals(ThemeCounter other)
        {
            return string.Equals(Theme, other.Theme) && string.Equals(CounterType, other.CounterType);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Theme?.GetHashCode() ?? 0) * 397) ^ (CounterType?.GetHashCode() ?? 0);
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Rikka.Tsab2.Core.Services.Analytics.ThemeCounter" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(ThemeCounter left, ThemeCounter right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Rikka.Tsab2.Core.Services.Analytics.ThemeCounter" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(ThemeCounter left, ThemeCounter right)
        {
            return !Equals(left, right);
        }
    }

    public class ActionCounter: ICounter
    {
        public ActionCounter()
        {
            
        }

        public ActionCounter(string action, double count)
        {
            Action = action;
            Count = count;
        }

        public string Action { get; set; }
        public double Count { get; set; }
        public string CounterType { get; set; } = "ActionCounter";

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((ActionCounter) obj);
        }

        protected bool Equals(ActionCounter other)
        {
            return string.Equals(Action, other.Action) && string.Equals(CounterType, other.CounterType);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((Action?.GetHashCode() ?? 0) * 397) ^ (CounterType?.GetHashCode() ?? 0);
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Rikka.Tsab2.Core.Services.Analytics.ActionCounter" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(ActionCounter left, ActionCounter right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Rikka.Tsab2.Core.Services.Analytics.ActionCounter" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(ActionCounter left, ActionCounter right)
        {
            return !Equals(left, right);
        }
    }

    public class RelationalCounter: ICounter
    {
        public RelationalCounter()
        {
            
        }

        public RelationalCounter(string subject, string relationType, double count)
        {
            Subject = subject;
            RelationType = relationType;
            Count = count;
        }

        public string Subject { get; set; }
        public string RelationType { get; set; }
        public double Count { get; set; }
        public string CounterType { get; set; } = "RelationalCounter";

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <returns>true if the specified object  is equal to the current object; otherwise, false.</returns>
        /// <param name="obj">The object to compare with the current object. </param>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((RelationalCounter) obj);
        }

        protected bool Equals(RelationalCounter other)
        {
            return string.Equals(Subject, other.Subject) && string.Equals(RelationType, other.RelationType) && string.Equals(CounterType, other.CounterType);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Subject?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (RelationType?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (CounterType?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:Rikka.Tsab2.Core.Services.Analytics.RelationalCounter" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(RelationalCounter left, RelationalCounter right)
        {
            return Equals(left, right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:Rikka.Tsab2.Core.Services.Analytics.RelationalCounter" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(RelationalCounter left, RelationalCounter right)
        {
            return !Equals(left, right);
        }
    }
    public class BaseAnalytics
    {
        public BaseAnalytics()
        {
           Counters = new List<ICounter>();
        }
        public List<ICounter> Counters{ get; set; }
    }

    public interface IAnalyticFetcherService
    {
        BaseAnalytics Fetch(IEnumerable<BaseAnalytics> analytics);

        IEnumerable<ICounter> Fetch(IEnumerable<BaseAnalytics> analytics,
            Expression<Func<DataAnalyticsType, string>> typeExpression);

        EmotionalCounter Fetch(IEnumerable<EmotionalCounter> counters);
    }

    public class AnalyticFetcherService : IAnalyticFetcherService
    {
        public BaseAnalytics Fetch(IEnumerable<BaseAnalytics> analytics)
        {
            var counters = new List<ICounter>();
            foreach (var item in analytics)
            {
                counters.AddRange(item.Counters);
            }
            return new BaseAnalytics()
            {
                Counters = counters
            };
        }

        public IEnumerable<ICounter> Fetch(IEnumerable<BaseAnalytics> analytics, Expression<Func<DataAnalyticsType, string>> typeExpression)
        {
            var typeBody = typeExpression.Compile();
            var type = typeBody.Invoke(new DataAnalyticsType());
            var counters = Fetch(analytics).Counters;
            return counters.Where(w => w.CounterType == type);

        }

        public EmotionalCounter Fetch(IEnumerable<EmotionalCounter> counters)
        {
            //var result = new EmotionalCounter();
            throw new NotImplementedException();
        }
    }
}
