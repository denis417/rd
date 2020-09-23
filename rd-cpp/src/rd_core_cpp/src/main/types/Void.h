#ifndef RD_CPP_VOID_H
#define RD_CPP_VOID_H

#include <functional>
#include <string>

#include <rd_core_export.h>

namespace rd
{
/**
 * \brief For using in idle events
 */
class RD_CORE_API Void
{
	friend bool RD_CORE_API operator==(const Void& lhs, const Void& rhs);

	friend bool RD_CORE_API operator!=(const Void& lhs, const Void& rhs);
};

std::string RD_CORE_API to_string(Void const&);
}	 // namespace rd

namespace std
{
template <>
struct hash<rd::Void>
{
	size_t operator()(const rd::Void&) const noexcept
	{
		return 0;
	}
};
}	 // namespace std

#endif	  // RD_CPP_VOID_H
