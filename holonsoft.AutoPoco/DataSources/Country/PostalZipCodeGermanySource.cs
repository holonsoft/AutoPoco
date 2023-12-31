﻿using holonsoft.AutoPoco.Configuration;
using holonsoft.AutoPoco.DataSources.Base;

namespace holonsoft.AutoPoco.DataSources.Country;
public abstract class PostalZipCodeGermanySourceBase(int? nullCreationThreshold = null) : FixedArrayWithStringsSourceBase(nullCreationThreshold) {
   protected override string[] Data => _dePostalZipCodes;

   private static readonly string[] _dePostalZipCodes = {
         "01067",
         "01069",
         "01097",
         "01099",
         "01108",
         "01109",
         "01127",
         "01129",
         "01139",
         "01156",
         "01157",
         "01159",
         "06255",
         "06258",
         "06259",
         "06268",
         "06279",
         "13627",
         "13629",
         "14050",
         "14052",
         "14053",
         "14055",
         "14057",
         "14059",
         "19395",
         "19399",
         "19406",
         "19412",
         "19417",
         "20095",
         "20097",
         "20099",
         "20144",
         "20146",
         "20148",
         "20149",
         "20249",
         "20251",
         "20253",
         "20255",
         "20257",
         "20259",
         "29664",
         "29683",
         "29690",
         "29693",
         "29699",
         "30159",
         "30161",
         "30163",
         "30165",
         "30167",
         "30169",
         "39638",
         "39646",
         "39649",
         "40210",
         "40211",
         "40212",
         "40213",
         "40215",
         "40217",
         "40219",
         "40221",
         "40223",
         "40225",
         "40227",
         "40229",
         "99891",
         "99894",
         "99897",
         "99947",
         "99955",
         "99958",
         "99974",
         "99976",
         "99986",
         "99988",
         "99991",
         "99994",
         "99996",
         "99998",
         "99999",
   };
}

public class PostalZipCodeGermanySource : PostalZipCodeGermanySourceBase {
   public PostalZipCodeGermanySource() : base() { }
}

public class NullablePostalZipCodeGermanySource : PostalZipCodeGermanySourceBase {
   public NullablePostalZipCodeGermanySource() : base(AutoPocoGlobalSettings.NullCreationThreshold) { }

   public NullablePostalZipCodeGermanySource(int nullCreationThreshold) : base(nullCreationThreshold) { }
}

