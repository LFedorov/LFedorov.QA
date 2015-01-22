using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Domain.Common.Command;
using Domain.Common.IoC;
using Domain.Common.Persistence;
using Domain.Common.Query;
using Domain.Entities;
using Domain.Entities.Commands.AccountCommands;
using Domain.Entities.Commands.DisciplineCommands;
using Domain.Entities.Commands.QuestionCommands;
using Domain.Entities.Queries.AnswerQueries;
using Domain.Entities.Queries.DisciplineQueries;
using Domain.Entities.Queries.QuestionQueries;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Maps;
using LFedorov.Moodle;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace Presentation.ConsoleApp
{
    static class IoC
    {
        static IoC()
        {
            Configure();
        }

        private static void Configure()
        {
            var builder = new ContainerBuilder();
            builder.Register(e => Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2008
                    .ConnectionString(x => x.FromConnectionStringWithKey("Main"))
                )
                .Mappings(x => x.FluentMappings.AddFromAssemblyOf<AccountMap>())
                .ExposeConfiguration(cfg => new SchemaExport(cfg).Execute(false, true, false))
                .BuildSessionFactory())
                .SingleInstance();

            builder.Register(x => x.Resolve<ISessionFactory>().OpenSession()).InstancePerLifetimeScope();
            builder.RegisterType<ConsoleAppResolver>().As<IResolver>();
            builder.RegisterType<UnitOfWorkFactory>().As<IUnitOfWorkFactory>();
            builder.RegisterType<Repository>().As<IRepository>();

            builder.RegisterAssemblyTypes(typeof(DisciplineQueryHandler).Assembly)
                .InNamespaceOf<DisciplineQueryHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(QuestionQueryHandler).Assembly)
                .InNamespaceOf<QuestionQueryHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(AnswerQueryHandler).Assembly)
                .InNamespaceOf<AnswerQueryHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(AccountCommandHandler).Assembly)
                .InNamespaceOf<AccountCommandHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(DisciplineCommandHandler).Assembly)
                .InNamespaceOf<DisciplineCommandHandler>()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(QuestionCommandHandler).Assembly)
                .InNamespaceOf<QuestionCommandHandler>()
                .AsImplementedInterfaces();

            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>();
            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>();

            Container = builder.Build();
        }

        public static IContainer Container { get; private set; }
    }

    class ConsoleAppResolver : IResolver
    {
        public object Resolve(Type type)
        {
            return IoC.Container.Resolve(type);
        }
    }

    class Program
    {
        static readonly Parser parser = new Parser();

        static void Main()
        {
            ReadQuestionFromFiles();

            var cmd = IoC.Container.Resolve<ICommandDispatcher>();
            cmd.Send(new CreateAccountCommand("fedorov86@gmail.com", "m478yx150"));

            ShowStat();
            Console.ReadLine();
        }

        private static void ReadQuestionFromFiles()
        {
            var files = Directory.EnumerateFiles(@"C:\Users\LFedorov\МГИУ\!Вопросы\", "*.*", SearchOption.AllDirectories)
                .Where(file => file.ToLower().EndsWith("mht") || file.ToLower().EndsWith("htm"))
                .ToList();

            var errors = new List<string>();

            foreach (var file in files)
            {
                try
                {
                    using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var questions = new List<Question>();

                        if (file.EndsWith("htm"))
                        {
                            questions = parser.GetContent(stream, "text/html");
                        }
                        else if (file.EndsWith("mht"))
                        {
                            questions = parser.GetContent(stream, "application/mime");
                        }

                        //var questionWithNullText = questions.Count(question => string.IsNullOrEmpty(question.Text));
                        //if (questionWithNullText > 0)
                        //{
                        //    Console.WriteLine("{0} has emptytext questions", file);
                        //}

                        if (questions.Count == 0)
                        {
                            Console.WriteLine(file);
                        }

                        var cmd = IoC.Container.Resolve<ICommandDispatcher>();
                        cmd.Send(new SaveQuestionsCommand(questions));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR IN FILE : {0} \n {1} \n {2} \n", file, e.Message, e.StackTrace);
                    //errors.Add(string.Format("Error in {0}", file));
                }
            }

            Console.WriteLine("Total: {0}", files.Count());

            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }

        private static void CleanData()
        {
            var repository = IoC.Container.Resolve<IRepository>();
            var uowFactory = IoC.Container.Resolve<IUnitOfWorkFactory>();
            using (var uow = uowFactory.Create())
            {
                foreach (var answer in repository.Query<Answer>().ToList())
                {
                    repository.Delete(answer);
                }

                foreach (var question in repository.Query<Question>().ToList())
                {
                    repository.Delete(question);
                }

                foreach (var discipline in repository.Query<Discipline>().ToList())
                {
                    repository.Delete(discipline);
                }

                uow.Commit();
            }
        }

        private static void DummyData()
        {
            var discipline = new Discipline("Discipline1");
            var q1 = new Question("Question1");
            var q2 = new Question("Question2");

            discipline.AddQuestion(q1);
            discipline.AddQuestion(q2);

            var a1 = new Answer("Answer1");
            var a2 = new Answer("Answer2");

            q1.AddAnswer(a1, true);
            q2.AddAnswer(a1, false);
            q1.AddAnswer(a2, false);

            var cmd = IoC.Container.Resolve<ICommandDispatcher>();
            cmd.Send(new SaveQuestionsCommand(discipline.Questions));

            var disciplines = new List<Discipline>();
            for (int i = 2; i < 20; i++)
            {
                disciplines.Add(new Discipline("Discipline " + i));
            }
            cmd.Send(new SaveDisciplinesCommand(disciplines));
        }

        static void ShowStat()
        {
            var handler = IoC.Container.Resolve<IQueryDispatcher>();
            var totalD = handler.Ask(new DisciplinesCountQuery());
            var totalQ = handler.Ask(new QuestionsCountQuery());
            var totalA = handler.Ask(new AnswersCountQuery());

            Console.WriteLine("Disciplines: {0}, Questions: {1}, Answers: {2}.", totalD, totalQ, totalA);

            //foreach (var discipline in handler.Ask(new DisciplinesQuery()))
            //{
            //    Console.WriteLine("{0} [{1}]", discipline.Id, discipline.Questions.Count());
            //}
        }
    }
}
