using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mediinfo.Infrastructure.Core.Domain
{
    /// <summary>
    /// 生成表达式目录树  泛型缓存
    /// </summary>
    /// <typeparam name="TIn"></typeparam>
    /// <typeparam name="TOut"></typeparam>
    public class ExpressionGenericMapper<TIn, TOut>//Mapper`2
    {        
        private static Func<TIn, TOut> _FUNC = null;
        static ExpressionGenericMapper()
        {
            string ErrorGetProperties = string.Empty;
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();
            foreach (var itemOut in typeof(TOut).GetProperties())
            {
                foreach (var itemIn in typeof(TIn).GetProperties())
                {
                    if (itemIn.Name == itemOut.Name)
                    {
                        try
                        {
                            MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(itemOut.Name));
                            MemberBinding memberBinding = Expression.Bind(itemOut, property);
                            memberBindingList.Add(memberBinding);
                        }
                        catch (Exception ex)
                        {
                            ErrorGetProperties += itemOut.Name +" ";
                            continue;
                        }
                    }
                }
            }

            //foreach (var item in typeof(TOut).GetFields())
            //{
            //    MemberExpression property = Expression.Field(parameterExpression, typeof(TIn).GetField(item.Name));
            //    MemberBinding memberBinding = Expression.Bind(item, property);
            //    memberBindingList.Add(memberBinding);
            //}
            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[]
            {
                    parameterExpression
            });
            _FUNC = lambda.Compile();//拼装是一次性的
            if (!string.IsNullOrEmpty(ErrorGetProperties))
            {
                throw new Exception($"{typeof(TIn).Name}与{typeof(TOut).Name}内的对应属性{ErrorGetProperties}类型存在不一致的情况，请仔细检查并修改，或者用辅助开发工具重新生成！");
            }
        }

        public static TOut Trans(TIn t)
        {
            return _FUNC(t);
        }
    }
}
