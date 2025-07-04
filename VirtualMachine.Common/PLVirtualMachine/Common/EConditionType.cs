﻿using System.ComponentModel;

namespace PLVirtualMachine.Common
{
  public enum EConditionType
  {
    [Description("TRUE")] CONDITION_TYPE_CONST_TRUE,
    [Description("FALSE")] CONDITION_TYPE_CONST_FALSE,
    [Description("LESS")] CONDITION_TYPE_VALUE_LESS,
    [Description("LESS_EQUAL")] CONDITION_TYPE_VALUE_LESS_EQUAL,
    [Description("LARGER")] CONDITION_TYPE_VALUE_LARGER,
    [Description("LARGER_EQUAL")] CONDITION_TYPE_VALUE_LARGER_EQUAL,
    [Description("EQUAL")] CONDITION_TYPE_VALUE_EQUAL,
    [Description("NOT_EQUAL")] CONDITION_TYPE_VALUE_NOT_EQUAL,
    [Description("EXPRESSION")] CONDITION_TYPE_VALUE_EXPRESSION,
  }
}
