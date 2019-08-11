using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Checkout.PaymentGateway.Api.Mvc.Validation;
using Xunit;

namespace Checkout.PaymentGateway.Api.Tests.Unit.Mvc.Validation
{
    public class NotDefaultAttributeTests
    {
        private readonly NotDefaultAttribute _attribute;

        public NotDefaultAttributeTests()
        {
            _attribute = new NotDefaultAttribute();
        }

        public static IEnumerable<object[]> Defaults => DefaultValues().Select(x => new []{x});
        public static IEnumerable<object[]> NonDefaults => NonDefaultValues().Select(x => new []{x});

        private static IEnumerable<object> DefaultValues()
        {
            yield return 0;
            yield return 0M;
            yield return 0D;
            yield return 0L;
            yield return Guid.Empty;
            yield return TestEnum.Default;
            yield return default(TestEnum);
        }

        private static IEnumerable<object> NonDefaultValues()
        {
            yield return "";
            yield return new { };
            yield return 1;
            yield return 1M;
            yield return 1D;
            yield return 1L;
            yield return double.NegativeInfinity;
            yield return double.PositiveInfinity;
            yield return double.Epsilon;
        }

        [Theory]
        [MemberData(nameof(Defaults))]
        public void WhenObjectIsADefaultValue_ThenIsValid(object o)
        {
            Assert.Throws<ValidationException>(() => _attribute.Validate(o, new ValidationContext(o)));
        }

        [Theory]
        [MemberData(nameof(NonDefaults))]
        public void WhenObjectIsNotADefaultValue_ThenIsNotValid(object o)
        {
            _attribute.Validate(o, new ValidationContext(o));
        }

        private enum TestEnum
        {
            Default,
            One,
            Two
        }
    }
}
