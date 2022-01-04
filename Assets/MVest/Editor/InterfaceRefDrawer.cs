using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization.Editor;
using Sirenix.OdinInspector;
using System.Reflection;
using System;

namespace MVest {
public class InterfaceRefAttributeProcessor<TInterface> : OdinAttributeProcessor<InterfaceRef<TInterface>> where TInterface : class {

    public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {
        attributes.Add(new RequireInterfaceAttribute(typeof(TInterface)));
    }

}
}

