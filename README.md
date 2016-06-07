# db4oviewer
This project *db4o viewer* provides **db4o** database file view without model classes assembly. It bases on **Db4objects** library.

This project tries to fix the bug:

    [InvalidOperationException: Unsupported class hierarchy change. Class System.Reflection.TypeInfo, mscorlib was added to hierarchy of System.RuntimeType, mscorlib]
       Db4objects.Db4o.Internal.Metadata.HierarchyAnalyzer.ThrowUnsupportedAdd(IReflectClass runtimeAncestor) +106
       Db4objects.Db4o.Internal.Metadata.HierarchyAnalyzer.Analyze() +208
       Db4objects.Db4o.Internal.ClassMetadata.CompareAncestorHierarchy() +49
       Db4objects.Db4o.Internal.ClassMetadata.DetectAspectTraversalStrategy() +15
       Db4objects.Db4o.Internal.ClassMetadata.TraverseAllAspects(ITraverseAspectCommand command) +24
       Db4objects.Db4o.Internal.ClassMetadataRepository.AttachQueryNode(String fieldName, IVisitor4 visitor) +110
       Db4objects.Db4o.Internal.Query.Processor.QCon.Attach(QQuery query, String a_field) +254
       Db4objects.Db4o.Internal.Query.Processor.QQueryBase.Descend1(QQuery query, String fieldName, IntByRef run) +189
       Db4objects.Db4o.Internal.Query.Processor.QQueryBase.Descend(String a_field) +131
       Unionless.Data.Db4o.AdViewDao.getByProductId(IObjectContainer db, Object[] arguments) in E:\WorkSpace\Code\TFS\Unionless\web_new\Data\Unionless.Data.Db4o\AdViewDao.cs:20
       Unionless.Data.Db4o.AbstractDao.ProcessQuery(QueryCallback callback, Object[] arguments) in E:\WorkSpace\Code\TFS\Unionless\web_new\Data\Unionless.Data.Db4o\AbstractDao.cs:30
       Unionless.Data.Db4o.AdViewDao.GetByProductId(String id) in E:\WorkSpace\Code\TFS\Unionless\web_new\Data\Unionless.Data.Db4o\AdViewDao.cs:14
       CompositionAopProxy_ce788fdf18374f4abd5a78e9941ecb8c.GetByProductId(String id) +196
       Admin_AdView2.OnInitializeControls(EventArgs e) +161
       Spring.Web.UI.Page.OnInit(EventArgs e) in c:\_prj\spring-net\trunk\src\Spring\Spring.Web\Web\UI\Page.cs:307
       Unionless.Web.SimpleBasePages.BasePage.OnInit(EventArgs e) +506
       Unionless.Web.SimpleBasePages.AdminPage.OnInit(EventArgs e) +49
       System.Web.UI.Control.InitRecursive(Control namingContainer) +134
       System.Web.UI.Page.ProcessRequestMain(Boolean includeStagesBeforeAsyncPoint, Boolean includeStagesAfterAsyncPoint) +489
