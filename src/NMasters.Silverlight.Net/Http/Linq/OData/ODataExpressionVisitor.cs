﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NMasters.Silverlight.Net.Http.Linq.OData
{
    internal class ODataVisitor : ExpressionVisitor
    {
        private StringBuilder _sb;

        internal ODataVisitor()
        {
        }

        internal string Translate(Expression expression)
        {
            _sb = new StringBuilder();
            expression = Evaluator.PartialEval(expression);
            Visit(expression);
            return _sb.ToString();
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }



        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(System.Linq.Queryable))
            {
                ChainOfResponsibility<MethodCallExpression>
                    .Start(m1 => m1.Method.Name == "Where", HandleWhere)
                    .Then(m1 => m1.Method.Name == "Take", HandleTake)
                    .Then(m1 => m1.Method.Name == "Skip", HandleSkip)
                    .Then(m1 => m1.Method.Name == "OrderBy", HandleOrderBy)
                    .Then(m1 => m1.Method.Name == "OrderByDescending", HandleOrderByDescending)
                    .Else(m1 =>
                    {
                        throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
                    })
                            .Run(m);
            }
            else
            {
                ChainOfResponsibility<MethodCallExpression>
                .Start(m1 => m1.Method.Name == "Substring", HandleSubstring)
                .Else(m1 =>
                {
                    throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
                })
                .Run(m);

            }


            return m;
        }

        private void HandleWhere(MethodCallExpression m)
        {
            this.Visit(m.Arguments[0]);
            AddAndIfNeeded();
            _sb.Append("$filter=");
            LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
            this.Visit(lambda.Body);
        }

        private void HandleTake(MethodCallExpression m)
        {
            this.Visit(m.Arguments[0]);
            AddAndIfNeeded();
            _sb.Append("$top=");
            this.Visit(m.Arguments[1]);
        }

        private void HandleSkip(MethodCallExpression m)
        {
            this.Visit(m.Arguments[0]);
            AddAndIfNeeded();
            _sb.Append("$skip=");
            this.Visit(m.Arguments[1]);
        }

        private void HandleOrderBy(MethodCallExpression m)
        {
            this.Visit(m.Arguments[0]);
            AddAndIfNeeded();
            _sb.Append("$orderby=");
            LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
            this.Visit(lambda.Body);
        }

        private void HandleOrderByDescending(MethodCallExpression m)
        {
            this.Visit(m.Arguments[0]);
            AddAndIfNeeded();
            _sb.Append("$orderby=");
            LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
            this.Visit(lambda.Body);
            _sb.Append(" DESC");

        }

        private void HandleSubstring(MethodCallExpression m)
        {
            _sb.Append("substring(");
            this.Visit(m.Object);
            _sb.Append(",");
            this.Visit(m.Arguments[0]);
            _sb.Append(",");
            this.Visit(m.Arguments[1]);
            _sb.Append(")");
        }

        private void AddAndIfNeeded()
        {
            if (_sb.Length > 0)
                _sb.Append("&");
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    _sb.Append(" not ");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            _sb.Append("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    _sb.Append(" eq ");
                    break;
                case ExpressionType.NotEqual:
                    _sb.Append(" ne ");
                    break;
                case ExpressionType.LessThan:
                    _sb.Append(" lt ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    _sb.Append(" le ");
                    break;
                case ExpressionType.GreaterThan:
                    _sb.Append(" gt ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    _sb.Append(" ge ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            _sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            var q = c.Value as IQueryable;
            if (q != null)
            {

                var innerExpression = q.Expression;
                if (innerExpression.NodeType == ExpressionType.Call)
                {
                    VisitMethodCall(innerExpression as MethodCallExpression);
                }
                //throw new NotSupportedException("Constant from IQueryable not supported."); // TODO: lool at this scenario
            }
            else if (c.Value == null)
            {
                _sb.Append("NULL");
            }
            else
            {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        _sb.Append(((bool)c.Value) ? 1 : 0);
                        break;
                    case TypeCode.String:
                        _sb.Append("'");
                        _sb.Append(c.Value);
                        _sb.Append("'");
                        break;
                    case TypeCode.Object:
                        throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", c.Value));
                    default:
                        _sb.Append(c.Value);
                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if (m.Expression == null)
                throw new ArgumentNullException("m.Expression");

            if (m.Expression.NodeType == ExpressionType.Parameter)
            {
                _sb.Append(m.Member.Name);
                return m;
            }
            else if (m.Expression.NodeType == ExpressionType.Constant)
            {
                // sb.Append(((ConstantExpression) m.Expression).Value);
                var value = ((ConstantExpression)m.Expression).Value;
                Visit(m.Expression);
                return m;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda<T>(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return base.VisitConditional(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            return base.VisitExtension(node);
        }

        public override Expression Visit(Expression node)
        {
            return base.Visit(node);
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }
    }
}