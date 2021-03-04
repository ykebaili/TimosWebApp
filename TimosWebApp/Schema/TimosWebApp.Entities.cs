
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;

using Aspectize.Core;

[assembly:AspectizeDALAssemblyAttribute]

namespace TimosWebApp
{
	public static partial class SchemaNames
	{
		public static partial class Entities
		{
			public const string Todos = "Todos";
			public const string User = "User";
			public const string ChampTimos = "ChampTimos";
			public const string ValeursChamp = "ValeursChamp";
			public const string TodoValeurChamp = "TodoValeurChamp";
			public const string DocumentsAttendus = "DocumentsAttendus";
			public const string FichiersAttaches = "FichiersAttaches";
			public const string GroupeChamps = "GroupeChamps";
			public const string Caracteristiques = "Caracteristiques";
			public const string CaracValeurChamp = "CaracValeurChamp";
			public const string Action = "Action";
		}

		public static partial class Relations
		{
			public const string ValeursPossibles = "ValeursPossibles";
			public const string RelationTodoGroupeChamps = "RelationTodoGroupeChamps";
			public const string RelationTodoValeurChamp = "RelationTodoValeurChamp";
			public const string RelationTodoDocument = "RelationTodoDocument";
			public const string RelationFichiers = "RelationFichiers";
			public const string RelationGroupeChampsChampsTimos = "RelationGroupeChampsChampsTimos";
			public const string RelationTodoCaracteristique = "RelationTodoCaracteristique";
			public const string RelationCaracChamp = "RelationCaracChamp";
			public const string RelationCaracValeurChamp = "RelationCaracValeurChamp";
			public const string RelationCaracValeursPossibles = "RelationCaracValeursPossibles";
			public const string RelationTodoActions = "RelationTodoActions";
		}
	}

	[SchemaNamespace]
	public class DomainProvider : INamespace
	{
		public string Name { get { return GetType().Namespace; } }
		public static string DomainName { get { return new DomainProvider().Name; } }
	}


	[DataDefinition(MustPersist = false)]
	public enum TypeDonnee
	{
		[Description("TypeEntier")]
		TypeEntier,
		[Description("TypeDecimal")]
		TypeDecimal		 = 		1,
		[Description("TypeString")]
		TypeString		 = 		2,
		[Description("TypeDate")]
		TypeDate		 = 		3,
		[Description("TypeBool")]
		TypeBool		 = 		4,
		[Description("ObjetTimos")]
		ObjetTimos		 = 		5
	}

	[DataDefinition(MustPersist = false)]
	public enum EtatTodo
	{
		[Description("EnAttente")]
		EnAttente,
		[Description("ADemarrer")]
		ADemarrer		 = 		1,
		[Description("Demarre")]
		Demarre		 = 		2,
		[Description("Termine")]
		Termine		 = 		3,
		[Description("Erreur")]
		Erreur		 = 		4,
		[Description("Annule")]
		Annule		 = 		5,
		[Description("EnRetard")]
		EnRetard		 = 		6
	}

	[DataDefinition]
	public class Todos : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Label = "Label";
			public const string StartDate = "StartDate";
			public const string Instructions = "Instructions";
			public const string TimosId = "TimosId";
			public const string ElementType = "ElementType";
			public const string ElementId = "ElementId";
			public const string ElementDescription = "ElementDescription";
			public const string EtatTodo = "EtatTodo";
			public const string EndDate = "EndDate";
			public const string DureeStandard = "DureeStandard";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);

		}

		[Data]
		public string Label
		{
			get { return getValue<string>("Label"); }
			set { setValue<string>("Label", value); }
		}

		[Data]
		public DateTime StartDate
		{
			get { return getValue<DateTime>("StartDate"); }
			set { setValue<DateTime>("StartDate", value); }
		}

		[Data]
		public string Instructions
		{
			get { return getValue<string>("Instructions"); }
			set { setValue<string>("Instructions", value); }
		}

		[Data(IsPrimaryKey = true)]
		public int TimosId
		{
			get { return getValue<int>("TimosId"); }
			set { setValue<int>("TimosId", value); }
		}

		[Data]
		public string ElementType
		{
			get { return getValue<string>("ElementType"); }
			set { setValue<string>("ElementType", value); }
		}

		[Data]
		public int ElementId
		{
			get { return getValue<int>("ElementId"); }
			set { setValue<int>("ElementId", value); }
		}

		[Data]
		public string ElementDescription
		{
			get { return getValue<string>("ElementDescription"); }
			set { setValue<string>("ElementDescription", value); }
		}

		[Data(DefaultValue = 0)]
		public EtatTodo EtatTodo
		{
			get { return getValue<EtatTodo>("EtatTodo"); }
			set { setValue<EtatTodo>("EtatTodo", value); }
		}

		[Data(IsNullable = true)]
		public DateTime? EndDate
		{
			get { return getValue<DateTime?>("EndDate"); }
			set { setValue<DateTime?>("EndDate", value); }
		}

		[Data]
		public int DureeStandard
		{
			get { return getValue<int>("DureeStandard"); }
			set { setValue<int>("DureeStandard", value); }
		}

	}

	[DataDefinition]
	public class User : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Name = "Name";
			public const string IsAuthentificated = "IsAuthentificated";
			public const string Login = "Login";
			public const string TimosKey = "TimosKey";
			public const string TimosSessionId = "TimosSessionId";
			public const string IsAdministrator = "IsAdministrator";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey=true)]
		public Guid Id
		{
			get { return getValue<Guid>("Id"); }
			set { setValue<Guid>("Id", value); }
		}

		[Data]
		public string Name
		{
			get { return getValue<string>("Name"); }
			set { setValue<string>("Name", value); }
		}

		[Data(DefaultValue = false)]
		public bool IsAuthentificated
		{
			get { return getValue<bool>("IsAuthentificated"); }
			set { setValue<bool>("IsAuthentificated", value); }
		}

		[Data]
		public string Login
		{
			get { return getValue<string>("Login"); }
			set { setValue<string>("Login", value); }
		}

		[Data]
		public string TimosKey
		{
			get { return getValue<string>("TimosKey"); }
			set { setValue<string>("TimosKey", value); }
		}

		[Data(DefaultValue = -1)]
		public int TimosSessionId
		{
			get { return getValue<int>("TimosSessionId"); }
			set { setValue<int>("TimosSessionId", value); }
		}

		[Data(DefaultValue = false)]
		public bool IsAdministrator
		{
			get { return getValue<bool>("IsAdministrator"); }
			set { setValue<bool>("IsAdministrator", value); }
		}

	}

	[DataDefinition]
	public class ChampTimos : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Nom = "Nom";
			public const string DisplayOrder = "DisplayOrder";
			public const string TimosId = "TimosId";
			public const string TypeDonneChamp = "TypeDonneChamp";
			public const string AspectizeControlType = "AspectizeControlType";
			public const string LibelleConvivial = "LibelleConvivial";
			public const string AspectizeFieldType = "AspectizeFieldType";
			public const string IsSelect = "IsSelect";
			public const string FormatDate = "FormatDate";
			public const string Editable = "Editable";
			public const string Multiline = "Multiline";
			public const string CustomClass = "CustomClass";
			public const string UseAutoComplete = "UseAutoComplete";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);

		}

		[Data]
		public string Nom
		{
			get { return getValue<string>("Nom"); }
			set { setValue<string>("Nom", value); }
		}

		[Data]
		public int DisplayOrder
		{
			get { return getValue<int>("DisplayOrder"); }
			set { setValue<int>("DisplayOrder", value); }
		}

		[Data(IsPrimaryKey = true)]
		public int TimosId
		{
			get { return getValue<int>("TimosId"); }
			set { setValue<int>("TimosId", value); }
		}

		[Data(DefaultValue = 2)]
		public TypeDonnee TypeDonneChamp
		{
			get { return getValue<TypeDonnee>("TypeDonneChamp"); }
			set { setValue<TypeDonnee>("TypeDonneChamp", value); }
		}

		[Data]
		public string AspectizeControlType
		{
			get { return getValue<string>("AspectizeControlType"); }
			set { setValue<string>("AspectizeControlType", value); }
		}

		[Data]
		public string LibelleConvivial
		{
			get { return getValue<string>("LibelleConvivial"); }
			set { setValue<string>("LibelleConvivial", value); }
		}

		[Data]
		public string AspectizeFieldType
		{
			get { return getValue<string>("AspectizeFieldType"); }
			set { setValue<string>("AspectizeFieldType", value); }
		}

		[Data(DefaultValue = false)]
		public bool IsSelect
		{
			get { return getValue<bool>("IsSelect"); }
			set { setValue<bool>("IsSelect", value); }
		}

		[Data]
		public string FormatDate
		{
			get { return getValue<string>("FormatDate"); }
			set { setValue<string>("FormatDate", value); }
		}

		[Data(DefaultValue = true)]
		public bool Editable
		{
			get { return getValue<bool>("Editable"); }
			set { setValue<bool>("Editable", value); }
		}

		[Data(DefaultValue = false)]
		public bool Multiline
		{
			get { return getValue<bool>("Multiline"); }
			set { setValue<bool>("Multiline", value); }
		}

		[Data]
		public string CustomClass
		{
			get { return getValue<string>("CustomClass"); }
			set { setValue<string>("CustomClass", value); }
		}

		[Data(DefaultValue = false)]
		public bool UseAutoComplete
		{
			get { return getValue<bool>("UseAutoComplete"); }
			set { setValue<bool>("UseAutoComplete", value); }
		}

	}

	[DataDefinition]
	public class ValeursChamp : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Index = "Index";
			public const string StoredValue = "StoredValue";
			public const string DisplayedValue = "DisplayedValue";
			public const string ChampTimosId = "ChampTimosId";
			public const string Id = "Id";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data]
		public int Index
		{
			get { return getValue<int>("Index"); }
			set { setValue<int>("Index", value); }
		}

		[Data]
		public string StoredValue
		{
			get { return getValue<string>("StoredValue"); }
			set { setValue<string>("StoredValue", value); }
		}

		[Data]
		public string DisplayedValue
		{
			get { return getValue<string>("DisplayedValue"); }
			set { setValue<string>("DisplayedValue", value); }
		}

		[Data]
		public string ChampTimosId
		{
			get { return getValue<string>("ChampTimosId"); }
			set { setValue<string>("ChampTimosId", value); }
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

	}

	[DataDefinition]
	public class TodoValeurChamp : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string ValeurChamp = "ValeurChamp";
			public const string LibelleChamp = "LibelleChamp";
			public const string OrdreChamp = "OrdreChamp";
			public const string ChampTimosId = "ChampTimosId";
			public const string ElementType = "ElementType";
			public const string ElementId = "ElementId";
			public const string Id = "Id";
			public const string UseAutoComplete = "UseAutoComplete";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data]
		public string ValeurChamp
		{
			get { return getValue<string>("ValeurChamp"); }
			set { setValue<string>("ValeurChamp", value); }
		}

		[Data]
		public string LibelleChamp
		{
			get { return getValue<string>("LibelleChamp"); }
			set { setValue<string>("LibelleChamp", value); }
		}

		[Data]
		public int OrdreChamp
		{
			get { return getValue<int>("OrdreChamp"); }
			set { setValue<int>("OrdreChamp", value); }
		}

		[Data]
		public int ChampTimosId
		{
			get { return getValue<int>("ChampTimosId"); }
			set { setValue<int>("ChampTimosId", value); }
		}

		[Data]
		public string ElementType
		{
			get { return getValue<string>("ElementType"); }
			set { setValue<string>("ElementType", value); }
		}

		[Data]
		public int ElementId
		{
			get { return getValue<int>("ElementId"); }
			set { setValue<int>("ElementId", value); }
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data(DefaultValue = false)]
		public bool UseAutoComplete
		{
			get { return getValue<bool>("UseAutoComplete"); }
			set { setValue<bool>("UseAutoComplete", value); }
		}

	}

	[DataDefinition]
	public class DocumentsAttendus : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Libelle = "Libelle";
			public const string TimosId = "TimosId";
			public const string IdCategorie = "IdCategorie";
			public const string NombreMin = "NombreMin";
			public const string DateLastUpload = "DateLastUpload";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data]
		public string Libelle
		{
			get { return getValue<string>("Libelle"); }
			set { setValue<string>("Libelle", value); }
		}

		[Data(IsPrimaryKey = true)]
		public int TimosId
		{
			get { return getValue<int>("TimosId"); }
			set { setValue<int>("TimosId", value); }
		}

		[Data]
		public int IdCategorie
		{
			get { return getValue<int>("IdCategorie"); }
			set { setValue<int>("IdCategorie", value); }
		}

		[Data]
		public int NombreMin
		{
			get { return getValue<int>("NombreMin"); }
			set { setValue<int>("NombreMin", value); }
		}

		[Data(IsNullable = true)]
		public DateTime? DateLastUpload
		{
			get { return getValue<DateTime?>("DateLastUpload"); }
			set { setValue<DateTime?>("DateLastUpload", value); }
		}

	}

	[DataDefinition]
	public class FichiersAttaches : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string NomFichier = "NomFichier";
			public const string TimosKey = "TimosKey";
			public const string DateUpload = "DateUpload";
			public const string DateDocument = "DateDocument";
			public const string Commentaire = "Commentaire";
			public const string DocumentId = "DocumentId";
			public const string CheminTemporaire = "CheminTemporaire";
			public const string Extension = "Extension";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data]
		public string NomFichier
		{
			get { return getValue<string>("NomFichier"); }
			set { setValue<string>("NomFichier", value); }
		}

		[Data(IsPrimaryKey = true)]
		public string TimosKey
		{
			get { return getValue<string>("TimosKey"); }
			set { setValue<string>("TimosKey", value); }
		}

		[Data]
		public DateTime DateUpload
		{
			get { return getValue<DateTime>("DateUpload"); }
			set { setValue<DateTime>("DateUpload", value); }
		}

		[Data(IsNullable = true)]
		public DateTime? DateDocument
		{
			get { return getValue<DateTime?>("DateDocument"); }
			set { setValue<DateTime?>("DateDocument", value); }
		}

		[Data]
		public string Commentaire
		{
			get { return getValue<string>("Commentaire"); }
			set { setValue<string>("Commentaire", value); }
		}

		[Data]
		public int DocumentId
		{
			get { return getValue<int>("DocumentId"); }
			set { setValue<int>("DocumentId", value); }
		}

		[Data]
		public string CheminTemporaire
		{
			get { return getValue<string>("CheminTemporaire"); }
			set { setValue<string>("CheminTemporaire", value); }
		}

		[Data]
		public string Extension
		{
			get { return getValue<string>("Extension"); }
			set { setValue<string>("Extension", value); }
		}

	}

	[DataDefinition]
	public class GroupeChamps : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Titre = "Titre";
			public const string OrdreAffichage = "OrdreAffichage";
			public const string TimosId = "TimosId";
			public const string Expand = "Expand";
			public const string InfosSecondaires = "InfosSecondaires";
			public const string CanAddCaracteristiques = "CanAddCaracteristiques";
			public const string TitreCaracteristiques = "TitreCaracteristiques";
			public const string LibelleChampAutoComplete = "LibelleChampAutoComplete";
			public const string IdChampAutoComplete = "IdChampAutoComplete";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data]
		public string Titre
		{
			get { return getValue<string>("Titre"); }
			set { setValue<string>("Titre", value); }
		}

		[Data]
		public int OrdreAffichage
		{
			get { return getValue<int>("OrdreAffichage"); }
			set { setValue<int>("OrdreAffichage", value); }
		}

		[Data(IsPrimaryKey = true)]
		public int TimosId
		{
			get { return getValue<int>("TimosId"); }
			set { setValue<int>("TimosId", value); }
		}

		[Data(DefaultValue = false)]
		public bool Expand
		{
			get { return getValue<bool>("Expand"); }
			set { setValue<bool>("Expand", value); }
		}

		[Data(DefaultValue = false)]
		public bool InfosSecondaires
		{
			get { return getValue<bool>("InfosSecondaires"); }
			set { setValue<bool>("InfosSecondaires", value); }
		}

		[Data(DefaultValue = false)]
		public bool CanAddCaracteristiques
		{
			get { return getValue<bool>("CanAddCaracteristiques"); }
			set { setValue<bool>("CanAddCaracteristiques", value); }
		}

		[Data]
		public string TitreCaracteristiques
		{
			get { return getValue<string>("TitreCaracteristiques"); }
			set { setValue<string>("TitreCaracteristiques", value); }
		}

		[Data(DefaultValue = "hidden")]
		public string LibelleChampAutoComplete
		{
			get { return getValue<string>("LibelleChampAutoComplete"); }
			set { setValue<string>("LibelleChampAutoComplete", value); }
		}

		[Data]
		public int IdChampAutoComplete
		{
			get { return getValue<int>("IdChampAutoComplete"); }
			set { setValue<int>("IdChampAutoComplete", value); }
		}

	}

	[DataDefinition]
	public class Caracteristiques : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string TimosId = "TimosId";
			public const string Titre = "Titre";
			public const string OrdreAffichage = "OrdreAffichage";
			public const string Expand = "Expand";
			public const string IdGroupePourFiltre = "IdGroupePourFiltre";
			public const string ElementType = "ElementType";
			public const string IsTemplate = "IsTemplate";
			public const string IdMetaType = "IdMetaType";
			public const string Id = "Id";
			public const string ParentElementType = "ParentElementType";
			public const string ParentElementId = "ParentElementId";
			public const string LibelleChampAutoComplete = "LibelleChampAutoComplete";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data]
		public int TimosId
		{
			get { return getValue<int>("TimosId"); }
			set { setValue<int>("TimosId", value); }
		}

		[Data]
		public string Titre
		{
			get { return getValue<string>("Titre"); }
			set { setValue<string>("Titre", value); }
		}

		[Data]
		public int OrdreAffichage
		{
			get { return getValue<int>("OrdreAffichage"); }
			set { setValue<int>("OrdreAffichage", value); }
		}

		[Data]
		public bool Expand
		{
			get { return getValue<bool>("Expand"); }
			set { setValue<bool>("Expand", value); }
		}

		[Data]
		public int IdGroupePourFiltre
		{
			get { return getValue<int>("IdGroupePourFiltre"); }
			set { setValue<int>("IdGroupePourFiltre", value); }
		}

		[Data]
		public string ElementType
		{
			get { return getValue<string>("ElementType"); }
			set { setValue<string>("ElementType", value); }
		}

		[Data]
		public bool IsTemplate
		{
			get { return getValue<bool>("IsTemplate"); }
			set { setValue<bool>("IsTemplate", value); }
		}

		[Data]
		public int IdMetaType
		{
			get { return getValue<int>("IdMetaType"); }
			set { setValue<int>("IdMetaType", value); }
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data]
		public string ParentElementType
		{
			get { return getValue<string>("ParentElementType"); }
			set { setValue<string>("ParentElementType", value); }
		}

		[Data]
		public int ParentElementId
		{
			get { return getValue<int>("ParentElementId"); }
			set { setValue<int>("ParentElementId", value); }
		}

		[Data]
		public string LibelleChampAutoComplete
		{
			get { return getValue<string>("LibelleChampAutoComplete"); }
			set { setValue<string>("LibelleChampAutoComplete", value); }
		}

	}

	[DataDefinition]
	public class CaracValeurChamp : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string ValeurChamp = "ValeurChamp";
			public const string LibelleChamp = "LibelleChamp";
			public const string OrdreChamp = "OrdreChamp";
			public const string ChampTimosId = "ChampTimosId";
			public const string ElementType = "ElementType";
			public const string ElementId = "ElementId";
			public const string Id = "Id";
			public const string UseAutoComplete = "UseAutoComplete";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data]
		public string ValeurChamp
		{
			get { return getValue<string>("ValeurChamp"); }
			set { setValue<string>("ValeurChamp", value); }
		}

		[Data]
		public string LibelleChamp
		{
			get { return getValue<string>("LibelleChamp"); }
			set { setValue<string>("LibelleChamp", value); }
		}

		[Data]
		public int OrdreChamp
		{
			get { return getValue<int>("OrdreChamp"); }
			set { setValue<int>("OrdreChamp", value); }
		}

		[Data]
		public int ChampTimosId
		{
			get { return getValue<int>("ChampTimosId"); }
			set { setValue<int>("ChampTimosId", value); }
		}

		[Data]
		public string ElementType
		{
			get { return getValue<string>("ElementType"); }
			set { setValue<string>("ElementType", value); }
		}

		[Data]
		public int ElementId
		{
			get { return getValue<int>("ElementId"); }
			set { setValue<int>("ElementId", value); }
		}

		[Data(IsPrimaryKey = true)]
		public string Id
		{
			get { return getValue<string>("Id"); }
			set { setValue<string>("Id", value); }
		}

		[Data(DefaultValue = false)]
		public bool UseAutoComplete
		{
			get { return getValue<bool>("UseAutoComplete"); }
			set { setValue<bool>("UseAutoComplete", value); }
		}

	}

	[DataDefinition]
	public class Action : Entity, IDataWrapper
	{
		public static partial class Fields
		{
			public const string Id = "Id";
			public const string Libelle = "Libelle";
			public const string Instructions = "Instructions";
			public const string IDT1 = "IDT1";
			public const string LBLT1 = "LBLT1";
			public const string VALT1 = "VALT1";
			public const string IDT2 = "IDT2";
			public const string LBLT2 = "LBLT2";
			public const string VALT2 = "VALT2";
			public const string IDT3 = "IDT3";
			public const string LBLT3 = "LBLT3";
			public const string VALT3 = "VALT3";
			public const string IDT4 = "IDT4";
			public const string LBLT4 = "LBLT4";
			public const string VALT4 = "VALT4";
			public const string IDT5 = "IDT5";
			public const string LBLT5 = "LBLT5";
			public const string VALT5 = "VALT5";
			public const string IDN1 = "IDN1";
			public const string LBLN1 = "LBLN1";
			public const string VALN1 = "VALN1";
			public const string IDN2 = "IDN2";
			public const string LBLN2 = "LBLN2";
			public const string VALN2 = "VALN2";
			public const string IDN3 = "IDN3";
			public const string LBLN3 = "LBLN3";
			public const string VALN3 = "VALN3";
			public const string IDD1 = "IDD1";
			public const string LBLD1 = "LBLD1";
			public const string VALD1 = "VALD1";
			public const string IDD2 = "IDD2";
			public const string LBLD2 = "LBLD2";
			public const string VALD2 = "VALD2";
			public const string IDD3 = "IDD3";
			public const string LBLD3 = "LBLD3";
			public const string VALD3 = "VALD3";
			public const string IDB1 = "IDB1";
			public const string VALB1 = "VALB1";
			public const string IDB2 = "IDB2";
			public const string LBLB2 = "LBLB2";
			public const string VALB2 = "VALB2";
			public const string IDB3 = "IDB3";
			public const string LBLB3 = "LBLB3";
			public const string VALB3 = "VALB3";
			public const string LBLB1 = "LBLB1";
		}

		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[Data(IsPrimaryKey = true)]
		public int Id
		{
			get { return getValue<int>("Id"); }
			set { setValue<int>("Id", value); }
		}

		[Data]
		public string Libelle
		{
			get { return getValue<string>("Libelle"); }
			set { setValue<string>("Libelle", value); }
		}

		[Data]
		public string Instructions
		{
			get { return getValue<string>("Instructions"); }
			set { setValue<string>("Instructions", value); }
		}

		[Data]
		public string IDT1
		{
			get { return getValue<string>("IDT1"); }
			set { setValue<string>("IDT1", value); }
		}

		[Data]
		public string LBLT1
		{
			get { return getValue<string>("LBLT1"); }
			set { setValue<string>("LBLT1", value); }
		}

		[Data]
		public string VALT1
		{
			get { return getValue<string>("VALT1"); }
			set { setValue<string>("VALT1", value); }
		}

		[Data]
		public string IDT2
		{
			get { return getValue<string>("IDT2"); }
			set { setValue<string>("IDT2", value); }
		}

		[Data]
		public string LBLT2
		{
			get { return getValue<string>("LBLT2"); }
			set { setValue<string>("LBLT2", value); }
		}

		[Data]
		public string VALT2
		{
			get { return getValue<string>("VALT2"); }
			set { setValue<string>("VALT2", value); }
		}

		[Data]
		public string IDT3
		{
			get { return getValue<string>("IDT3"); }
			set { setValue<string>("IDT3", value); }
		}

		[Data]
		public string LBLT3
		{
			get { return getValue<string>("LBLT3"); }
			set { setValue<string>("LBLT3", value); }
		}

		[Data]
		public string VALT3
		{
			get { return getValue<string>("VALT3"); }
			set { setValue<string>("VALT3", value); }
		}

		[Data]
		public string IDT4
		{
			get { return getValue<string>("IDT4"); }
			set { setValue<string>("IDT4", value); }
		}

		[Data]
		public string LBLT4
		{
			get { return getValue<string>("LBLT4"); }
			set { setValue<string>("LBLT4", value); }
		}

		[Data]
		public string VALT4
		{
			get { return getValue<string>("VALT4"); }
			set { setValue<string>("VALT4", value); }
		}

		[Data]
		public string IDT5
		{
			get { return getValue<string>("IDT5"); }
			set { setValue<string>("IDT5", value); }
		}

		[Data]
		public string LBLT5
		{
			get { return getValue<string>("LBLT5"); }
			set { setValue<string>("LBLT5", value); }
		}

		[Data]
		public string VALT5
		{
			get { return getValue<string>("VALT5"); }
			set { setValue<string>("VALT5", value); }
		}

		[Data]
		public string IDN1
		{
			get { return getValue<string>("IDN1"); }
			set { setValue<string>("IDN1", value); }
		}

		[Data]
		public string LBLN1
		{
			get { return getValue<string>("LBLN1"); }
			set { setValue<string>("LBLN1", value); }
		}

		[Data]
		public int VALN1
		{
			get { return getValue<int>("VALN1"); }
			set { setValue<int>("VALN1", value); }
		}

		[Data]
		public string IDN2
		{
			get { return getValue<string>("IDN2"); }
			set { setValue<string>("IDN2", value); }
		}

		[Data]
		public string LBLN2
		{
			get { return getValue<string>("LBLN2"); }
			set { setValue<string>("LBLN2", value); }
		}

		[Data]
		public int VALN2
		{
			get { return getValue<int>("VALN2"); }
			set { setValue<int>("VALN2", value); }
		}

		[Data]
		public string IDN3
		{
			get { return getValue<string>("IDN3"); }
			set { setValue<string>("IDN3", value); }
		}

		[Data]
		public string LBLN3
		{
			get { return getValue<string>("LBLN3"); }
			set { setValue<string>("LBLN3", value); }
		}

		[Data]
		public int VALN3
		{
			get { return getValue<int>("VALN3"); }
			set { setValue<int>("VALN3", value); }
		}

		[Data]
		public string IDD1
		{
			get { return getValue<string>("IDD1"); }
			set { setValue<string>("IDD1", value); }
		}

		[Data]
		public string LBLD1
		{
			get { return getValue<string>("LBLD1"); }
			set { setValue<string>("LBLD1", value); }
		}

		[Data]
		public DateTime VALD1
		{
			get { return getValue<DateTime>("VALD1"); }
			set { setValue<DateTime>("VALD1", value); }
		}

		[Data]
		public string IDD2
		{
			get { return getValue<string>("IDD2"); }
			set { setValue<string>("IDD2", value); }
		}

		[Data]
		public string LBLD2
		{
			get { return getValue<string>("LBLD2"); }
			set { setValue<string>("LBLD2", value); }
		}

		[Data]
		public DateTime VALD2
		{
			get { return getValue<DateTime>("VALD2"); }
			set { setValue<DateTime>("VALD2", value); }
		}

		[Data]
		public string IDD3
		{
			get { return getValue<string>("IDD3"); }
			set { setValue<string>("IDD3", value); }
		}

		[Data]
		public string LBLD3
		{
			get { return getValue<string>("LBLD3"); }
			set { setValue<string>("LBLD3", value); }
		}

		[Data]
		public DateTime VALD3
		{
			get { return getValue<DateTime>("VALD3"); }
			set { setValue<DateTime>("VALD3", value); }
		}

		[Data]
		public string IDB1
		{
			get { return getValue<string>("IDB1"); }
			set { setValue<string>("IDB1", value); }
		}

		[Data]
		public bool VALB1
		{
			get { return getValue<bool>("VALB1"); }
			set { setValue<bool>("VALB1", value); }
		}

		[Data]
		public string IDB2
		{
			get { return getValue<string>("IDB2"); }
			set { setValue<string>("IDB2", value); }
		}

		[Data]
		public string LBLB2
		{
			get { return getValue<string>("LBLB2"); }
			set { setValue<string>("LBLB2", value); }
		}

		[Data]
		public bool VALB2
		{
			get { return getValue<bool>("VALB2"); }
			set { setValue<bool>("VALB2", value); }
		}

		[Data]
		public string IDB3
		{
			get { return getValue<string>("IDB3"); }
			set { setValue<string>("IDB3", value); }
		}

		[Data]
		public string LBLB3
		{
			get { return getValue<string>("LBLB3"); }
			set { setValue<string>("LBLB3", value); }
		}

		[Data]
		public bool VALB3
		{
			get { return getValue<bool>("VALB3"); }
			set { setValue<bool>("VALB3", value); }
		}

		[Data]
		public string LBLB1
		{
			get { return getValue<string>("LBLB1"); }
			set { setValue<string>("LBLB1", value); }
		}

	}

	[DataDefinition]
	public class ValeursPossibles : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(ValeursChamp), Role = typeof(ValeursChamp), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity ValeursChamp;

		[RelationEnd(Type = typeof(GroupeChamps), Role = typeof(GroupeChamps), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity GroupeChamps;

	}

	[DataDefinition]
	public class RelationTodoGroupeChamps : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Todos), Role = typeof(Todos), Multiplicity = Multiplicity.ZeroOrOne)]
		public IEntity Todos;

		[RelationEnd(Type = typeof(GroupeChamps), Role = typeof(GroupeChamps), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity GroupeChamps;

	}

	[DataDefinition]
	public class RelationTodoValeurChamp : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(TodoValeurChamp), Role = typeof(TodoValeurChamp), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity TodoValeurChamp;

		[RelationEnd(Type = typeof(GroupeChamps), Role = typeof(GroupeChamps), Multiplicity = Multiplicity.ZeroOrOne)]
		public IEntity GroupeChamps;

	}

	[DataDefinition]
	public class RelationTodoDocument : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Todos), Role = typeof(Todos), Multiplicity = Multiplicity.One)]
		public IEntity Todos;

		[RelationEnd(Type = typeof(DocumentsAttendus), Role = typeof(DocumentsAttendus), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity DocumentsAttendus;

	}

	[DataDefinition]
	public class RelationFichiers : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(DocumentsAttendus), Role = typeof(DocumentsAttendus), Multiplicity = Multiplicity.One)]
		public IEntity DocumentsAttendus;

		[RelationEnd(Type = typeof(FichiersAttaches), Role = typeof(FichiersAttaches), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity FichiersAttaches;

	}

	[DataDefinition]
	public class RelationGroupeChampsChampsTimos : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(GroupeChamps), Role = typeof(GroupeChamps), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity GroupeChamps;

		[RelationEnd(Type = typeof(ChampTimos), Role = typeof(ChampTimos), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity ChampTimos;

	}

	[DataDefinition]
	public class RelationTodoCaracteristique : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Caracteristiques), Role = typeof(Caracteristiques), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Caracteristiques;

		[RelationEnd(Type = typeof(Todos), Role = typeof(Todos), Multiplicity = Multiplicity.ZeroOrOne)]
		public IEntity Todos;

	}

	[DataDefinition]
	public class RelationCaracChamp : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Caracteristiques), Role = typeof(Caracteristiques), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Caracteristiques;

		[RelationEnd(Type = typeof(ChampTimos), Role = typeof(ChampTimos), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity ChampTimos;

	}

	[DataDefinition]
	public class RelationCaracValeurChamp : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Caracteristiques), Role = typeof(Caracteristiques), Multiplicity = Multiplicity.ZeroOrOne)]
		public IEntity Caracteristiques;

		[RelationEnd(Type = typeof(CaracValeurChamp), Role = typeof(CaracValeurChamp), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity CaracValeurChamp;

	}

	[DataDefinition]
	public class RelationCaracValeursPossibles : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Caracteristiques), Role = typeof(Caracteristiques), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Caracteristiques;

		[RelationEnd(Type = typeof(ValeursChamp), Role = typeof(ValeursChamp), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity ValeursChamp;

	}

	[DataDefinition]
	public class RelationTodoActions : DataWrapper, IDataWrapper, IRelation
	{
		void IDataWrapper.InitData(DataRow data, string namePrefix)
		{
			base.InitData(data, null);
		}

		[RelationEnd(Type = typeof(Todos), Role = typeof(Todos), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Todos;

		[RelationEnd(Type = typeof(Action), Role = typeof(Action), Multiplicity = Multiplicity.ZeroOrMany)]
		public IEntity Action;

	}

}


  
