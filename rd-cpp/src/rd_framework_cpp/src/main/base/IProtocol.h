//
// Created by jetbrains on 30.07.2018.
//

#ifndef RD_CPP_IPROTOCOL_H
#define RD_CPP_IPROTOCOL_H


#include "IRdDynamic.h"
#include "Serializers.h"
#include "Identities.h"
#include "IScheduler.h"
#include "IWire.h"

#include <memory>

namespace rd {
	//region predeclared

	class SerializationCtx;
	//endregion

	class IProtocol : public IRdDynamic {
	public:
		std::unique_ptr<Serializers> serializers = std::make_unique<Serializers>();
		std::shared_ptr<Identities> identity;
		IScheduler *scheduler = nullptr;
		std::shared_ptr<IWire> wire;

		//region ctor/dtor

		IProtocol();

		IProtocol(std::shared_ptr<Identities> identity, IScheduler *scheduler, std::shared_ptr<IWire> wire);

		IProtocol(IProtocol &&other) noexcept = default;

		IProtocol &operator=(IProtocol &&other) noexcept = default;

		virtual ~IProtocol();
		//endregion
	};
}


#endif //RD_CPP_IPROTOCOL_H
