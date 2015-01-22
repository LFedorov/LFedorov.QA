using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Domain.Common.Command;
using Domain.Common.IoC;
using Domain.Common.Persistence;
using Domain.Common.Query;
using Domain.Entities.Commands.AccountCommands;
using Domain.Entities.Commands.DisciplineCommands;
using Domain.Entities.Commands.QuestionCommands;
using Domain.Entities.Queries.AccountQueries;
using Domain.Entities.Queries.AnswerQueries;
using Domain.Entities.Queries.DisciplineQueries;
using Domain.Entities.Queries.QuestionQueries;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Maps;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Presentation.Web
{
    public class IoCConfig
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            var database = MsSqlConfiguration.MsSql2008
                //.Driver<StackExchange.Profiling.NHibernate.Drivers.MiniProfilerSql2008ClientDriver>()
                .ConnectionString(x => x.FromConnectionStringWithKey("Main"));

            builder.Register(e => Fluently.Configure()
                .Database(database)
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<AccountMap>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .BuildSessionFactory())
                .SingleInstance();

            builder.Register(x => x.Resolve<ISessionFactory>().OpenSession()).InstancePerRequest();
            builder.RegisterType<WebResolver>().As<IResolver>();
            builder.RegisterType<UnitOfWorkFactory>().As<IUnitOfWorkFactory>();
            builder.RegisterType<Repository>().As<IRepository>();

            #region CommandHandlers
            builder.RegisterAssemblyTypes(typeof(AccountCommandHandler).Assembly)
                .InNamespaceOf<AccountCommandHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(DisciplineCommandHandler).Assembly)
                .InNamespaceOf<DisciplineCommandHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(QuestionCommandHandler).Assembly)
                .InNamespaceOf<QuestionCommandHandler>()
                .AsImplementedInterfaces();
            #endregion

            #region QueryHandlers
            builder.RegisterAssemblyTypes(typeof(AccountQueryHandler).Assembly)
                .InNamespaceOf<AccountQueryHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(DisciplineQueryHandler).Assembly)
                .InNamespaceOf<DisciplineQueryHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(QuestionQueryHandler).Assembly)
                .InNamespaceOf<QuestionQueryHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(AnswerQueryHandler).Assembly)
                .InNamespaceOf<AnswerQueryHandler>()
                .AsImplementedInterfaces();
            #endregion

            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>();
            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}