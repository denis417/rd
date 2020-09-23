#ifndef RD_CPP_VIEWABLE_COLLECTIONS_H
#define RD_CPP_VIEWABLE_COLLECTIONS_H

#include <string>

#include <rd_core_export.h>

namespace rd
{
enum class AddRemove
{
	ADD,
	REMOVE
};

std::string RD_CORE_API to_string(AddRemove kind);

enum class Op
{
	ADD,
	UPDATE,
	REMOVE,
	ACK
};

std::string RD_CORE_API to_string(Op op);
}	 // namespace rd

#endif	  // RD_CPP_VIEWABLE_COLLECTIONS_H
