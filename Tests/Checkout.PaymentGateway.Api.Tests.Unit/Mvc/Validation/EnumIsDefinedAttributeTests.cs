using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Checkout.PaymentGateway.Api.Mvc.Validation;
using Xunit;

namespace Checkout.PaymentGateway.Api.Tests.Unit.Mvc.Validation
{
    public class EnumIsDefinedAttributeTests
    {
        private readonly EnumIsDefinedAttribute _attribute;

        public EnumIsDefinedAttributeTests()
        {
            _attribute = new EnumIsDefinedAttribute();
        }

        public static IEnumerable<object[]> Valid => ValidValues().Select(x => new []{x});
        public static IEnumerable<object[]> Invalid => InvalidValues().Select(x => new []{x});

        private static IEnumerable<object> ValidValues()
        {
            yield return default(TestEnum);
            yield return TestEnum.Default;
            yield return TestEnum.One;
            yield return TestEnum.Two;
        }

        private static IEnumerable<object> InvalidValues()
        {
            yield return (TestEnum)(-1);
            yield return (TestEnum)(1000);
        }

        [Theory]
        [MemberData(nameof(Valid))]
        public void WhenObjectIsADefinedValue_ThenIsValid(object o)
        {
            _attribute.Validate(o, new ValidationContext(o));
        }

        [Theory]
        [MemberData(nameof(Invalid))]
        public void WhenObjectIsNotADefinedValue_ThenIsNotValid(object o)
        {
            Assert.Throws<ValidationException>(() => _attribute.Validate(o, new ValidationContext(o)));
        }

        private enum TestEnum
        {
            Default,
            One,
            Two
        }
    }
}